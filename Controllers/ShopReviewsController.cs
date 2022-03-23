using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using Catel.Data;
using GNT_server.Models;

namespace GNT_server.Controllers
{
    [RoutePrefix("api/shopreviews")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ShopReviewsController : ApiController
    {
        private projectDBEntities db = new projectDBEntities();
        [Route("")]
        [HttpGet]
        // GET: api/ShopReviews
        public IQueryable<ShopReview> GetShopReview()
        {
            return db.ShopReview;
        }

        // GET: api/ShopReviews/5
        [Route("{memberid:int}")] //搜尋會員評分紀錄
        [HttpGet]
        [ResponseType(typeof(ShopReview))]
        public IHttpActionResult GetShopReview(int memberid)
        {
            var shopReview = db.ShopReview.Where(s => s.MemberID == memberid);
            var shopReviewCount = db.ShopReview.Where(s => s.MemberID == memberid).ToList();
            if (shopReviewCount.Count == 0)
            {
                //return NotFound();
                //return Content(HttpStatusCode.NotFound, "無此ID"); //錯誤訊息
                return BadRequest("無此會員");

            }
            return Ok(shopReview);
        }

        [Route("keywords/{keywords}")] //模糊查尋會員評分內容
        [HttpGet]
        [ResponseType(typeof(ShopReview))]
        public IHttpActionResult GetShopReviewKeyWord(string keywords)
        {
            var shopReview = db.ShopReview.Where(s => s.RContent.Contains(keywords));
            var shopReviewCount = db.ShopReview.Where(s => s.RContent.Contains(keywords)).ToList();
            if (shopReviewCount.Count == 0)
            {
                //return NotFound();
                //return Content(HttpStatusCode.NotFound, "無此ID"); //錯誤訊息
                return BadRequest("無此資料");

            }
            return Ok(shopReview);
        }

        // PUT: api/ShopReviews/5
        [Route("{memberid}/{shopid}")] //修改會員評分紀錄
        [HttpPut]
        
        [ResponseType(typeof(void))]
        public IHttpActionResult PutShopReview(int memberid, int shopid, ShopReview shopReview)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //if (memberid != shopReview.MemberID || shopid != shopReview.ShopID) //
            //{
            //    //return BadRequest();
            //    //return Content(HttpStatusCode.BadRequest, "會員ID或店家ID錯誤");
            //    return BadRequest("店家ID錯誤");
            //}

            db.Entry(shopReview).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShopReviewExists(memberid))
                {
                    //return NotFound();
                    return BadRequest("查無此會員紀錄");
                }
                else if (!ShopReviewExists(shopid)) {
                    return BadRequest("該會員無此店家評論");
                }
                else
                {
                    throw;
                }
            }
            //return Content(HttpStatusCode.OK, "修改完成");
            //return StatusCode(HttpStatusCode.NoContent);
            return Ok("修改完成");
        }

        // POST: api/ShopReviews
        [Route("")]
        [ResponseType(typeof(ShopReview))]
        public IHttpActionResult PostShopReview(ShopReview shopReview) //新增評論
        {
            //if (!ModelState.IsValid)
            if(shopReview.RContent == null)
            {
                return BadRequest("內容必填");
            }
            else if(shopReview.Score == null)
            {
                return BadRequest("評分必填");
            }

            db.ShopReview.Add(shopReview);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (ShopReviewExists(shopReview.MemberID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok("新增成功");//CreatedAtRoute("DefaultApi", new { id = shopReview.MemberID }, shopReview);

        }

        
        [Route("Admin/{memberid}/{shopid}")] //admin刪除會員評分紀錄
        [HttpDelete]
        [ResponseType(typeof(ShopReview))]
        public IHttpActionResult DeleteShopReviewAdmin(int memberid, int shopid)
        {
            ShopReview shopReview = db.ShopReview.Find(memberid, shopid);
            if (shopReview == null)
            {
                return BadRequest("查無此評分紀錄");
            }

            db.ShopReview.Remove(shopReview);
            db.SaveChanges();

            return Ok("刪除成功");
        }

        [Route("{memberid}/{shopid}")] //刪除會員評分紀錄
        [HttpDelete]
        [ResponseType(typeof(ShopReview))]
        public IHttpActionResult DeleteShopReviewMember(int memberid, int shopid)
        {
            ShopReview shopReview = db.ShopReview.Find(memberid, shopid);
            if (shopReview == null)
            {
                return BadRequest("查無此評分紀錄");
            }

            db.ShopReview.Remove(shopReview);
            db.SaveChanges();

            return Ok("刪除成功");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ShopReviewExists(int id)
        {
            return db.ShopReview.Count(e => e.MemberID == id) > 0;
        }
    }
}