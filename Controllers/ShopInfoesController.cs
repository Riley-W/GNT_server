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
using LinqKit;

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
        public IHttpActionResult GetAllShopInfo()
        {
            string tags = "";
            int iint = 0;
            var result = from s in db.ShopInfo
                         select s;

            foreach(var a in result)
            {
                if (!string.IsNullOrEmpty(a.TagIds))
                {
                    string[] ids = a.TagIds.Split(',');
                    foreach (string i in ids)
                    {
                        Int32.TryParse(i, out iint);
                        var tagname = from t in db.Tag
                                      where t.TagID == iint
                                      select t.TagName;
                       foreach(string tn in tagname)
                        {
                            tags = tn + ",";
                        }
                        if (tags.Trim().Substring(tags.Trim().Length - 1, 1) == ",")
                        {
                            tags = tags.Trim().Substring(0, tags.Trim().Length - 1);
                        }
                    }
                    a.TagIds = tags;
                }
                
            }
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
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
                realtype = "小吃宵夜";
            else if (type == "dessert")
                realtype = "咖啡甜點";
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


        /// <summary>
        /// 查詢店家依據 店名、地址、tag、type (前台)
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="address"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]//待更正為動態生成
        [Route("search")]//api/ShopInfoes/search?tag=2,5
        [Obsolete]
        public IHttpActionResult GetShopInfoTag(string tag,string address, string name,string type)
        {
            var alldata = PredicateBuilder.True<ShopInfo>();
            if (!string.IsNullOrWhiteSpace(address))
            {
                alldata = alldata.And(a => a.Address.Contains(address));
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                alldata = alldata.And(a => a.Name.Contains(name));
            }
            if (!string.IsNullOrWhiteSpace(type))
            {
                string realtype;
                if (type == "bar")
                    realtype = "酒吧";
                else if (type == "snack")
                    realtype = "小吃宵夜";
                else if (type == "dessert")
                    realtype = "咖啡甜點";
                else if (type == "viewpoint")
                    realtype = "夜間景點";
                else
                    realtype = "";
                if(realtype!="")
                    alldata = alldata.And(a => a.Type.Contains(realtype));
            }
            if (!string.IsNullOrWhiteSpace(tag))
            {
                string[] Tag = tag.Split(',');
                if (Tag != null)
                {
                    foreach (var Qint in Tag)
                    {
                        alldata = alldata.And(a => a.TagIds.Contains(Qint));

                    }

                }
            
                var result = db.ShopInfo.Where(alldata);

                return Ok(result);
            }
            return NotFound();
        }
        //    PUT: api/ShopInfoes/5
        //     api/{controller}/{id}
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
        // POST: api/ShopInfoes
        //api/{controller}
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