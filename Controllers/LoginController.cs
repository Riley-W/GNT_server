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

namespace GNT_server.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LoginController : ApiController
    {

        [HttpPost]
        public Object Post(AdminInfo admininfo)
        {
            projectDBEntities db = new projectDBEntities();
            var result = (from a in db.AdminInfo
                          where a.Account == admininfo.Account
                          && a.Password == admininfo.Password
                          select a).FirstOrDefault();
            if (result!=null)
            {
                JwtAuthUtil jwtAuthUtil = new JwtAuthUtil();
                string jwtToken = jwtAuthUtil.GenerateToken(admininfo.Account);
                return new
                {
                    status = true,
                    message="登入成功",
                    expiretime = ExpiredTime.ETime,
                    token = jwtToken
                    
                };
            }
            else
            {
                return new
                {
                    status = false,
                    token = "Account Or Password Error"
                };
            }
        }
    }
}
