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
    
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ShopReviewsController : ApiController
    {
        private projectDBEntities db = new projectDBEntities();
        
        // GET: api/ShopReviews
        public IQueryable<ShopReview> GetShopReview()
        {
            return db.ShopReview;
        }

        // GET: api/ShopReviews/5
        [Route("api/shopreviews/{memberid}")] //搜尋會員評分紀錄
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
                return BadRequest("無此ID");

            }
            return Ok(shopReview);
        }

        // PUT: api/ShopReviews/5
        [Route("api/shopreviews/{memberid}/{shopid}")] //修改會員評分紀錄
        [HttpPut]
        
        [ResponseType(typeof(void))]
        public IHttpActionResult PutShopReview(int memberid, int shopid, ShopReview shopReview)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (memberid != shopReview.MemberID && shopid != shopReview.ShopID)
            {
                //return BadRequest();
                //return Content(HttpStatusCode.BadRequest, "會員ID或店家ID錯誤");
                return BadRequest("會員ID或店家ID錯誤");
            }

            db.Entry(shopReview).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShopReviewExists(memberid))
                {
                    return NotFound();
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
        [ResponseType(typeof(ShopReview))]
        public IHttpActionResult PostShopReview(ShopReview shopReview) //新增評論
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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

        // DELETE: api/ShopReviews/5
        [Route("api/shopreviews/{memberid}/{shopid}")] //刪除會員評分紀錄
        [HttpDelete]
        [ResponseType(typeof(ShopReview))]
        public IHttpActionResult DeleteShopReview(int memberid, int shopid)
        {
            ShopReview shopReview = db.ShopReview.Find(memberid, shopid);
            if (shopReview == null)
            {
                return NotFound();
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