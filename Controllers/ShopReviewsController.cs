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
        /// <summary>
        /// 查詢所有評論(後台)
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [HttpGet]
        // GET: api/ShopReviews
        public List<object> GetShopReview()
        {
            var shopReview = from s in db.ShopReview
                             orderby s.ReviewDate descending
                             select s;
            return ShopReviewTransfer.changetime(shopReview);
        }

        // GET: api/ShopReviews/5
        /// <summary>
        /// 查詢會員評論紀錄(前台)
        /// </summary>
        /// <param name="memberid"></param>
        /// <returns></returns>
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
                return BadRequest("無此會員評分紀錄");

            }
            //datelist = shopreviewtransform.changetime(shopReview);
            return Ok(ShopReviewTransfer.changetime(shopReview));
        }
        /// <summary>
        /// 關鍵字搜尋(後台)
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        [Route("keywords/{keywords}")] //模糊查尋會員評分內容
        [HttpGet]
        [ResponseType(typeof(ShopReview))]
        public IHttpActionResult GetShopReviewKeyWord(string keywords)
        {
            var shopReview = db.ShopReview.Where(s => s.RContent.Contains(keywords) );
            var shopReviewCount = db.ShopReview.Where(s => s.RContent.Contains(keywords)).ToList();
            if (shopReviewCount.Count == 0)
            {
                //return NotFound();
                //return Content(HttpStatusCode.NotFound, "無此ID"); //錯誤訊息
                return BadRequest("無此資料");

            }
            return Ok(ShopReviewTransfer.changetime(shopReview));
        }

        /// <summary>
        /// 計算店家平均分數(前台渲染)
        /// </summary>
        /// <param name="shopid"></param>
        /// <returns></returns>
        [Route("score/{shopid}")] //計算店家平均分數
        [HttpGet]
        [ResponseType(typeof(ShopReview))]
        public IHttpActionResult GetShopReviewScore(int shopid)
        {
            var shopreview = db.ShopReview.Where(s => s.ShopID == shopid).ToList();
            //return Ok(shopreview);
            var score = shopreview.Select(c => c.Score).Average();
            if (score == null)
            {
                return BadRequest("該店家無平均分數");
            }
            else
            {
                double aveScore = Math.Round((double)score, 1);
                return Ok(aveScore);
            }
        }

        // PUT: api/ShopReviews/5
        /// <summary>
        /// 會員修改評分紀錄(前台)
        /// </summary>
        /// <param name="memberid"></param>
        /// <param name="shopid"></param>
        /// <param name="shopReview"></param>
        /// <returns></returns>
        [Route("{memberid}/{shopid}")] //修改會員評分紀錄
        [HttpPut]

        [ResponseType(typeof(void))]
        public IHttpActionResult PutShopReview(int memberid, int shopid, ShopReview shopReview)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            shopReview.ReviewDate = DateTime.Now;
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
                else if (!ShopReviewExists(shopid))
                {
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
        /// <summary>
        /// 會員新增評論(前台)
        /// </summary>
        /// <param name="shopReview"></param>
        /// <returns></returns>
        [Route("")]
        [ResponseType(typeof(ShopReview))]
        public IHttpActionResult PostShopReview(ShopReview shopReview) //新增評論
        {
            //if (!ModelState.IsValid)
            if (shopReview.RContent == null)
            {
                return BadRequest("內容必填");
            }
            else if (shopReview.Score == null)
            {
                return BadRequest("評分必填");
            }
            shopReview.ReviewDate = DateTime.Now;
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

        /// <summary>
        /// Admin刪除會員評論(後台)
        /// </summary>
        /// <param name="memberid"></param>
        /// <param name="shopid"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 會員刪除評論(前台)
        /// </summary>
        /// <param name="memberid"></param>
        /// <param name="shopid"></param>
        /// <returns></returns>
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