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
using System.Web.Http.Description;
using GNT_server.Models;

namespace GNT_server.Controllers
{
    public class WebsiteReviewController : ApiController
    {
        private projectDBEntities1 db = new projectDBEntities1();

        // GET: WebsiteReview

        // POST: EDIT
        
        [ResponseType(typeof(WebsiteReview))]
        public async Task<IHttpActionResult> PostWebsiteReview(WebsiteReview WebsiteReview)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (WebsiteReview.MemberID == null || WebsiteReview.ReviewDate == null || WebsiteReview.Type == null || WebsiteReview.RContent == null || WebsiteReview.Status == null)
            {
                //驗證是否資料齊全 //未綁Remark
                return BadRequest(ModelState);
            }
            else
            {
                db.WebsiteReview.Add(WebsiteReview);
                await db.SaveChangesAsync();

                return CreatedAtRoute("DefaultApi", new { MemberID = WebsiteReview.MemberID, ReviewDate = WebsiteReview.ReviewDate, Type = WebsiteReview.Type, RContent = WebsiteReview.RContent, Status = WebsiteReview.Status}, WebsiteReview);
            }


        }

        public IQueryable<WebsiteReview> GetWebsiteReview()
        {   //QueryAll
            return db.WebsiteReview;

        }

        public IQueryable<WebsiteReview> GetWebsiteReviewByType(string Type)
        {   //QueryByType: 推薦店家 系統回饋 店家資訊更新 其他
            return db.WebsiteReview.Where(p => p.Type == Type);
        }

        public IQueryable<WebsiteReview> GetWebsiteReviewByStatus(string Status)
        {   //QueryByStatus: 已處理 處理中 未處理
            return db.WebsiteReview.Where(p => p.Status == Status);
        }

        public IQueryable<WebsiteReview> GetWebsiteReviewByKeywords(string Keywords)
        {   //QueryByKeywords: 

            return db.WebsiteReview.Where(p => p.ReviewDate.ToString().Contains(Keywords) || p.Type.Contains(Keywords) || p.RContent.Contains(Keywords) || p.Status.Contains(Keywords) || p.Remark.Contains(Keywords));
        }

        [Route("api/WebsiteReview/{ReviewID}")]
        [HttpGet]
        public IQueryable<WebsiteReview> GetWebsiteReviewByID(int ReviewID)
        {   //QueryByID
            var GetReviewByID = db.WebsiteReview.Where(p => p.ReviewID == ReviewID);

            if (GetReviewByID == null)
            {
                return (IQueryable<WebsiteReview>)NotFound();
            }
            return GetReviewByID;

        }


        //DELETE: 
        [Route("api/WebsiteReview/{ReviewID}")]
        [HttpDelete]
        [ResponseType(typeof(WebsiteReview))]
        public async Task<IHttpActionResult> DeleteWebsiteReview(int ReviewID)
        {
            //DELETE
            WebsiteReview websiteReviewWithID = await db.WebsiteReview.FindAsync(ReviewID);
            if (websiteReviewWithID != null)
            {
                BadRequest("找無此ID");
            }

            db.WebsiteReview.Remove(websiteReviewWithID);
            await db.SaveChangesAsync();

            return Ok(websiteReviewWithID);
        }


        // PUT: EDIT
        [Route("api/WebsiteReview/{ReviewID}")]
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
                return BadRequest();
            }

            if (WebsiteReview.MemberID == null || WebsiteReview.ReviewDate == null || WebsiteReview.Type == null || WebsiteReview.RContent == null || WebsiteReview.Status == null)
            {
                //驗證是否資料齊全
                return BadRequest("缺少資料");
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

            return Ok(WebsiteReview); //StatusCode(HttpStatusCode.OK);        

        }


        private bool WebsiteReviewExits(int ReviewID)
        {
            return db.WebsiteReview.Count(e => e.ReviewID == ReviewID) > 0;
        }

    }
}