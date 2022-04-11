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
            var result = from s in db.ShopInfo
                         select s;
            var tagname = from t in db.Tag
                          select t;
            var ChineseTagresult = ShopInfoTransfer.ChangeTagtoChinese(result, tagname);
            if (ChineseTagresult == null)
            {
                return Content(HttpStatusCode.NotFound, "查無此店家");
            }

            return Ok(ChineseTagresult);
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
            var result = from s in db.ShopInfo
                         where s.ShopID==id
                         select s;
            var tagname = from t in db.Tag
                          select t;
            var ChineseTagresult = ShopInfoTransfer.ChangeTagtoChinese(result, tagname);

            if (ChineseTagresult == null)
            {
                return Content(HttpStatusCode.NotFound, "查無此店家");
            }

            return Ok(ChineseTagresult);
        }
        /// <summary>
        /// 查詢四大分類店家(前台)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [Route("type/{type:length(1,50)}")]
        public IHttpActionResult GetShopInfoType(string type)
        {
            string realtype=ShopInfoTransfer.TypeTransfer(type);
            var result = from s in db.ShopInfo
                         where s.Type== realtype
                         select s;
            var tagname = from t in db.Tag
                          select t;
            var ChineseTagresult = ShopInfoTransfer.ChangeTagtoChinese(result, tagname);

            if (result == null)
            {
                return Content(HttpStatusCode.NotFound, "查無店家");
            }

            return Ok(result);
        }
        /// <summary>
        /// 查詢店家依據  Enable 請給bool
        /// </summary>
        /// <param name="isEnable"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Enable")]//api/ShopInfoes/Enable?isEnable=2,5
        public IHttpActionResult GetAllEnableShopInfo(bool isEnable)
        {
            var result = from s in db.ShopInfo
                         where s.Enable==isEnable
                         select s;
            var tagname = from t in db.Tag
                          select t;
            var ChineseTagresult=ShopInfoTransfer.ChangeTagtoChinese(result, tagname);

            
            if (ChineseTagresult == null)
            {
                return Content(HttpStatusCode.NotFound, "查無此店家");
            }

            return Ok(ChineseTagresult);
        }
        /// <summary>
        /// 查詢店家依據 店名、地址、tag、type (前台)
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="address"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("search")]//api/ShopInfoes/search?tag=2,5
        public IHttpActionResult GetShopInfoTag(string tag, string address, string name, string type)
        {
            var findwithouttag = PredicateBuilder.True<ShopInfo>();
            var findbytag = PredicateBuilder.False<ShopInfo>();
            if (address != "null")
            {
                findwithouttag = findwithouttag.And(a => a.Address.Contains(address));
            }
            if (name != "null")
            {
                findwithouttag = findwithouttag.And(a => a.Name.Contains(name));
            }
            if (type != "null")
            {
                string realtype = ShopInfoTransfer.TypeTransfer(type);
                if (realtype != "")
                    findwithouttag = findwithouttag.And(a => a.Type.Contains(realtype));
            }
            if (tag != "null")
            {
                string[] Tag = tag.Split(',');
                if (Tag != null)
                {
                    foreach (string Qint in Tag)
                    {
                        findbytag = findbytag.Or(a => a.TagIds.Contains(Qint));
                    }
                }
            }
            IQueryable<ShopInfo> result = null;
            if ((address== "null" && name== "null" && type== "null") && tag!= "null")
            {
                result = db.ShopInfo.AsExpandable().Where(findbytag);
            }
            else if ((address != "null" || name != "null" || type != "null") && tag == "null")
            {
                result = db.ShopInfo.AsExpandable().Where(findwithouttag);
            }
            else if ((address != "null" || name != "null" || type != "null") && tag != "null")
            {
                result = db.ShopInfo.AsExpandable().Where(findwithouttag).Where(findbytag);
            }
            else if ((address == "null" && name == "null" && type == "null") && tag == "null")
            {
                result = from s in db.ShopInfo
                         select s;
            }
            var tagname = from t in db.Tag
                          select t;

            var ChineseTagresult = ShopInfoTransfer.ChangeTagtoChinese(result, tagname);
            if (ChineseTagresult != null)
                return Ok(result);

             return Content(HttpStatusCode.NotFound, "查無店家");
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
        [Route("Admin/{id:int}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutShopInfoAdmin(int id, ShopInfo shopInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != shopInfo.ShopID)
            {
                return BadRequest();
            }
            var tagname = from t in db.Tag
                          select t;
            shopInfo.TagIds = ShopInfoTransfer.ChangeChinesetoTag(shopInfo, tagname);
            db.Entry(shopInfo).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShopInfoExists(id))
                {
                    return Content(HttpStatusCode.NotFound, "查無店家");
                }
                else
                {
                    throw;
                }
            }

            return Ok("修改成功");
        }
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
            var tagname = from t in db.Tag
                          select t;
            shopInfo.TagIds = ShopInfoTransfer.ChangeChinesetoTag(shopInfo, tagname);
            db.Entry(shopInfo).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShopInfoExists(id))
                {
                    return Content(HttpStatusCode.NotFound, "查無店家");
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
        [Route("Admin")]
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