using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using GNT_server.Models;

namespace GNT_server.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/websitereview")]
    public class WebsiteReviewController : ApiController
    {
        private projectDBEntities db = new projectDBEntities();

        // GET: WebsiteReview
        [Route("")]
        public IQueryable<WebsiteReview> GetWebsiteReview()
        {   //QueryAll
            return db.WebsiteReview;

        }

        [Route("type/{type}")]
        public IQueryable<WebsiteReview> GetWebsiteReviewByType(string Type)
        {   //QueryByType: 推薦店家 系統回饋 店家資訊更新 其他
            return db.WebsiteReview.Where(p => p.Type == Type);
        }

        [Route("status/{status}")]
        public IQueryable<WebsiteReview> GetWebsiteReviewByStatus(string Status)
        {   //QueryByStatus: 已處理 處理中 未處理
            return db.WebsiteReview.Where(p => p.Status == Status);
        }

        [Route("keywords/{keywords}")]
        public IQueryable<WebsiteReview> GetWebsiteReviewByKeywords(string Keywords)
        {   //QueryByKeywords: 

               return db.WebsiteReview.Where(p => p.ReviewDate.ToString().Contains(Keywords) || p.Type.Contains(Keywords) || p.RContent.Contains(Keywords) || p.Status.Contains(Keywords) || p.Remark.Contains(Keywords));

        }

        [Route("{ReviewID}")]
        [HttpGet]
        public IQueryable<WebsiteReview> GetWebsiteReviewByID(int ReviewID)
        {   //QueryByID
            var GetReviewByID = db.WebsiteReview.Where(p => p.ReviewID == ReviewID);
            var GetReviewByIDCount = db.WebsiteReview.Where(p => p.ReviewID == ReviewID).Count();
            if (GetReviewByIDCount < 1)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NoContent) { Content = new StringContent("找無此ID。") });
            }
            return GetReviewByID;

        }


        // POST: ADD
        [Route("")]
        [HttpPost]
        [ResponseType(typeof(WebsiteReview))]
        public async Task<IHttpActionResult> PostWebsiteReview(WebsiteReview WebsiteReview)
        {
            if (!ModelState.IsValid)
            {   
                return BadRequest(ModelState);
            }
            //if (WebsiteReview.MemberID == null || WebsiteReview.ReviewDate == null || WebsiteReview.Type == null || WebsiteReview.RContent == null)
            //{
            //    //驗證是否資料齊全 //未綁Remark
            //    return BadRequest("資料未完整。");
            //}
            else
            {
                WebsiteReview.Status = "未處理";
                db.WebsiteReview.Add(WebsiteReview);
                await db.SaveChangesAsync();

                //return CreatedAtRoute("DefaultApi", new { MemberID = WebsiteReview.MemberID, ReviewDate = WebsiteReview.ReviewDate, Type = WebsiteReview.Type, RContent = WebsiteReview.RContent, Status = WebsiteReview.Status}, WebsiteReview);
                return Ok("資料新增完成。");
            }


        }

        //DELETE: 
        [Route("admin/{ReviewID}")]
        [HttpDelete]
        [ResponseType(typeof(WebsiteReview))]
        public async Task<IHttpActionResult> DeleteWebsiteReview(int ReviewID)
        {
            //DELETE
            WebsiteReview websiteReviewWithID = await db.WebsiteReview.FindAsync(ReviewID);
            if (websiteReviewWithID != null)
            {
                BadRequest("找無此ID。");
            }

            db.WebsiteReview.Remove(websiteReviewWithID);
            await db.SaveChangesAsync();

            //return Ok(websiteReviewWithID);
            return Ok("資料已刪除。");
        }


        // PUT: EDIT
        [Route("admin/{ReviewID}")]
        [HttpPut]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutWebsiteReview(int ReviewID, WebsiteReview WebsiteReview) // id傳進來的資料, WebsiteReview修改的資料
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (ReviewID != WebsiteReview.ReviewID)
            {
                return BadRequest("找無對應ID。");
            }

            if (WebsiteReview.MemberID == null || WebsiteReview.ReviewDate == null || WebsiteReview.Type == null || WebsiteReview.RContent == null || WebsiteReview.Status == null)
            {
                //驗證是否資料齊全
                return BadRequest("資料未完整。");
            }

            db.Entry(WebsiteReview).State = EntityState.Modified;

            try 
            { 
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WebsiteReviewExits(ReviewID))
                {
                    return BadRequest("找無此ID");
                }
                else
                {
                    throw;
                }
            }

            //return Ok(WebsiteReview); //StatusCode(HttpStatusCode.OK);
            return Ok("修改完成。");

        }


        private bool WebsiteReviewExits(int ReviewID)
        {
            return db.WebsiteReview.Count(e => e.ReviewID == ReviewID) > 0;
        }

    }
}