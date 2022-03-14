using Jose;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Http.Filters;

namespace GNT_server.Security
{
    public class JwtAuthFilter : ActionFilterAttribute
{
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            string secret = "GoodNightTainanORoKaMoNo";//加解密的key,如果不一樣會無法成功解密
            var request = actionContext.Request;
            if (!WithoutVerifyToken(request.RequestUri.ToString()))
            {
                if (request.Headers.Authorization == null || request.Headers.Authorization.Scheme != "Bearer")
                {
                    throw new System.Exception("Lost Token");
                }
                else
                {
                    var jwtObject = Jose.JWT.Decode<Dictionary<string, Object>>(
                    request.Headers.Authorization.Parameter,
                    Encoding.UTF8.GetBytes(secret),
                    JwsAlgorithm.HS512);

                    if (IsTokenExpired(jwtObject["Exp"].ToString()))
                    {
                        throw new System.Exception("Token Expired");
                    }
                }
            }

            base.OnActionExecuting(actionContext);
        }

        //Login不需要驗證因為還沒有token
        public bool WithoutVerifyToken(string requestUri)
        {
            if (requestUri.EndsWith("/Login"))
                return true;
            else if(requestUri.Contains("Admin"))
                return false;
            return true;
        }

        //驗證token時效
        public bool IsTokenExpired(string dateTime)
        {
            return Convert.ToDateTime(dateTime) < DateTime.Now;
        }



        
    }
}