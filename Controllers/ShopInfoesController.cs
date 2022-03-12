using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using GNT_server.Models;

namespace GNT_server.Controllers
{
    [RoutePrefix("api/ShopInfoes")]
    public class ShopInfoesController : ApiController
    {
        private projectDBEntities db = new projectDBEntities();
        [Route("")]
        // GET: api/ShopInfoes
        public IQueryable<ShopInfo> GetShopInfo()
        {
            return db.ShopInfo;
        }

        // GET: api/ShopInfoes/5
        [Route("{id:int}")]
        [ResponseType(typeof(ShopInfo))]
        public IHttpActionResult GetShopInfo(int id)
        {
            ShopInfo shopInfo = db.ShopInfo.Find(id);
            if (shopInfo == null)
            {
                return NotFound();
            }

            return Ok(shopInfo);
        }
        [Route("type/{type:length(1,50)}")]
        public IHttpActionResult GetShopInfoType(string type)
        {
            string realtype;
            if (type == "bar")
                realtype = "酒吧";
            else if (type == "snack")
                realtype = "宵夜小吃";
            else if (type == "dessert")
                realtype = "深夜甜點";
            else if (type == "viewpoint")
                realtype = "夜間景點";
            else
                realtype = "";
            var shopInfo = from s in db.ShopInfo
                           where s.Type == realtype
                           select s;
            if (shopInfo == null)
            {
                return NotFound();
            }
            return Ok(shopInfo);
        }
        [Route("tag/{tag:length(1,50)}")]
        public IHttpActionResult GetShopInfoTag1(string tag)
        {
            var shopInfo = from s in db.ShopInfo
                           where s.Tag.Contains(tag)
                           select s;
            if (shopInfo == null)
            {
                return NotFound();
            }
            return Ok(shopInfo);
        }
        [Route("tag/{taga:length(1,50)}/{tagb:length(1,50)}")]
        public IHttpActionResult GetShopInfoTag2(string taga, string tagb)
        {
            var shopInfo = from s in db.ShopInfo
                           where s.Tag.Contains(taga)
                           && s.Tag.Contains(tagb)
                           select s;
            if (shopInfo == null)
            {
                return NotFound();
            }
            return Ok(shopInfo);
        }
        [Route("tag/{taga:length(1,50)}/{tagb:length(1,50)}/{tagc:length(1,50)}")]
        public IHttpActionResult GetShopInfoTag3(string taga, string tagb, string tagc)
        {
            var shopInfo = from s in db.ShopInfo
                           where s.Tag.Contains(taga)
                           && s.Tag.Contains(tagb)
                           && s.Tag.Contains(tagc)
                           select s;
            if (shopInfo == null)
            {
                return NotFound();
            }
            return Ok(shopInfo);
        }
        // PUT: api/ShopInfoes/5

        [HttpPut]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutShopInfo(int id, ShopInfo shopInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != shopInfo.ShopID)
            {
                return BadRequest();
            }

            db.Entry(shopInfo).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShopInfoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/ShopInfoes
        [HttpPost]
        [ResponseType(typeof(ShopInfo))]
        public IHttpActionResult PostShopInfo(ShopInfo shopInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ShopInfo.Add(shopInfo);
            db.SaveChanges();

            return CreatedAtRoute("PostApi", new { id = shopInfo.ShopID }, shopInfo);
        }

        // DELETE: api/ShopInfoes/5
        [HttpDelete]
        [ResponseType(typeof(ShopInfo))]
        public IHttpActionResult DeleteShopInfo(int id)
        {
            ShopInfo shopInfo = db.ShopInfo.Find(id);
            if (shopInfo == null)
            {
                return NotFound();
            }

            db.ShopInfo.Remove(shopInfo);
            db.SaveChanges();

            return Ok(shopInfo);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ShopInfoExists(int id)
        {
            return db.ShopInfo.Count(e => e.ShopID == id) > 0;
        }
    }
}