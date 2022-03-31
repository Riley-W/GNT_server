using GNT_server.Models;
using GNT_server.Security;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;

namespace GNT_server.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/Login")]
    public class LoginController : ApiController
    {
        /// <summary>
        /// Admin登入(後台)
        /// </summary>
        /// <param name="admininfo"></param>
        /// <returns></returns>
        [ResponseType(typeof(AdminInfo))]
        [HttpPost]
        [Route("AD")]
        public IHttpActionResult Post(AdminInfo admininfo)
        {
            projectDBEntities db = new projectDBEntities();
            var result = (from a in db.AdminInfo
                          where a.Account == admininfo.Account
                          && a.Password == admininfo.Password
                          select a).FirstOrDefault();
            if (result != null)
            {
                JwtAuthUtil jwtAuthUtil = new JwtAuthUtil();
                string jwtToken = jwtAuthUtil.GenerateToken(admininfo.Account);
                var loginmessage = new
                {
                    status = true,
                    message = "登入成功",
                    expiretime = ExpiredTime.ETime,
                    token = jwtToken

                };
                return Ok(loginmessage);
            }
            else
            {
                return BadRequest("登入失敗");
            }
        }


        /// <summary>
        /// Member登入(前台)
        /// </summary>
        /// <param name="memberinfo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Member")]
        
        public IHttpActionResult Post(MemberInfo memberinfo)
        {
            projectDBEntities db = new projectDBEntities();
            var result = (from m in db.MemberInfo
                          where m.Account == memberinfo.Account
                          && m.Password == memberinfo.Password
                          select m).FirstOrDefault();

            if (result != null)
            {
                var loginmessage = new
                {
                    status = true,
                    ID = result.MemberID,
                    message = "登入成功"
                };
                return Ok(loginmessage);
            }
            else
            {

                return BadRequest("登入失敗");
            }
        }
    }
}

