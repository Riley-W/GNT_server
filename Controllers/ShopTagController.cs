using GNT_server.Models;
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
    [RoutePrefix("api/shoptag")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ShopTagController : ApiController
    {
        private projectDBEntities db = new projectDBEntities();
        /// <summary>
        /// 查詢店家Tag(渲染)
        /// </summary>
        /// <param name="shopid"></param>
        /// <returns></returns>
        [Route("{shopid:int}")]
        [ResponseType(typeof(ShopInfo))]
        public IHttpActionResult GetShopInfo(int shopid)
        {
            var result = from s in db.ShopTag
                         join n in db.Tag on s.TagID equals n.TagID
                         where s.ShopID == shopid
                         select new 
                         {
                             Tag=s.TagID,
                             TagName=n.TagName
                         };
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
