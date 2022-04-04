using Jose;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace GNT_server.Security
{
    public class JwtAuthFilter : ActionFilterAttribute
{
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            string secret = "GoodNightTainan";//加解密的key,如果不一樣會無法成功解密
            var request = actionContext.Request;
            if (!WithoutVerifyToken(request.RequestUri.ToString()))
            {
                if (request.Headers.Authorization == null || request.Headers.Authorization.Scheme != "Bearer")
                {
                    setErrorResponse(actionContext, "驗證錯誤");
                }
                else
                {
                    try
                    {
                        var jwtObject = Jose.JWT.Decode<Dictionary<string, Object>>(
                        request.Headers.Authorization.Parameter,
                        Encoding.UTF8.GetBytes(secret),
                        JwsAlgorithm.HS512);
                        long exptime = 0;
                        Int64.TryParse(jwtObject["Exp"].ToString(), out exptime);
                        if (IsTokenExpired(exptime))
                        {
                            setErrorResponse(actionContext, "憑證過期");
                        }
                    }
                    catch (Exception e)
                    {
                        setErrorResponse(actionContext, e.Message);
                    }
                }
            }

            base.OnActionExecuting(actionContext);
        }

        private void setErrorResponse(HttpActionContext actionContext, string message)
        {
            var response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, message);
            actionContext.Response = response;
        }


        //Login不需要驗證因為還沒有token
        public bool WithoutVerifyToken(string requestUri)
        {
            if (requestUri.Contains("/Login"))
                return true;
            else if (requestUri.Contains("Admin"))
                return false;
            else if (requestUri.Contains("admin"))
                return false;
            return true;
        }

        //驗證token時效
        public bool IsTokenExpired(long dateTime)
        {
            DateTime expireddate = ExpiredTime.startTime.AddMilliseconds(dateTime);
            return Convert.ToDateTime(expireddate) < DateTime.Now;
        }



        
    }
}