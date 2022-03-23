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
using System.Web;
using System.Web.Http.Cors;

namespace GNT_server.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/ShopInfoes")]
    public class ShopInfoesController : ApiController
    {
        private projectDBEntities db = new projectDBEntities();
        /// <summary>
        /// 查詢所有店家(前台)
        /// </summary>
        /// <returns></returns>
        [Route("")]
        // GET: api/ShopInfoes
        public IQueryable<ShopInfo> GetShopInfo()
        {
            return db.ShopInfo;
        }

        // GET: api/ShopInfoes/5
        /// <summary>
        /// 查詢店家ID(前台)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 查詢四大分類店家(前台)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
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
        //[HttpGet]//待更正為動態生成
        //[Route("tag")]//api/ShopInfoes/tag?tag=2,5
        //public IHttpActionResult GetShopInfoTag(string tag)
        //{
        //    string Qstring= Query.QueryFromList(tag);
           
        //    var shopInfo = from s in db.ShopInfo
        //                   where s.Tag.Contains(Qstring)
        //                   select s;
        //    if (shopInfo == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(shopInfo);
        //}
        //[HttpGet]
        //[Route("typeandtag")]//api/ShopInfoes/typeandtag?type=bar&tag=2,5
        //public IHttpActionResult GetShopInfoTypeAndTag(string type , string tag)
        //{
        //    string Qstring = Query.QueryFromList(tag);
        //    string realtype;
        //    if (type == "bar")
        //        realtype = "酒吧";
        //    else if (type == "snack")
        //        realtype = "宵夜小吃";
        //    else if (type == "dessert")
        //        realtype = "深夜甜點";
        //    else if (type == "viewpoint")
        //        realtype = "夜間景點";
        //    else
        //        realtype = "";
        //    var shopInfo = from s in db.ShopInfo
        //                   where s.Tag.Contains(Qstring)
        //                   && s.Type==realtype
        //                   select s;
        //    if (shopInfo == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(shopInfo);
        //}
        // PUT: api/ShopInfoes/5
        // api/{controller}/update/{id}
        /// <summary>
        /// 修改店家(後台)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="shopInfo"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id:int}")]
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

            return Ok("修改成功");
        }
        //[HttpPatch]
        //[ResponseType(typeof(void))]
        //public IHttpActionResult PatchShopInfo(int id, ShopInfo shopInfo)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != shopInfo.ShopID)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(shopInfo).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ShopInfoExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        // POST: api/ShopInfoes
        //api/{controller}/create
        /// <summary>
        /// 新增店家(後台)
        /// </summary>
        /// <param name="shopInfo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
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
        //[HttpDelete]
        //[ResponseType(typeof(ShopInfo))]
        //public IHttpActionResult DeleteShopInfo(int id)
        //{
        //    ShopInfo shopInfo = db.ShopInfo.Find(id);
        //    if (shopInfo == null)
        //    {
        //        return NotFound();
        //    }

        //    db.ShopInfo.Remove(shopInfo);
        //    db.SaveChanges();

        //    return Ok(shopInfo);
        //}

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