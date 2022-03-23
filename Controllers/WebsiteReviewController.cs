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
using System.Net.Mail;//寄送mail
using System.Net.Mime;//寄送mail
using System.Configuration;


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

               // SendEmail();

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
                var mailAddress = db.MemberInfo.Find(WebsiteReview.MemberID).Email.ToString();
                var receiver = db.MemberInfo.Find(WebsiteReview.MemberID).Name.ToString();
                SendEmail(mailAddress, receiver, WebsiteReview.RContent);
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

        private void SendEmail(string mailAddress, string receiver, string reviewContent)
        {
            try
            {
                SmtpClient mySmtpClient = new SmtpClient("smtp.gmail.com", 587);

                //set smtp-client with basicAuthentication

                //NetworkCredential LoginInfo = new NetworkCredential("goodnighttainan@gmail.com", "P@ssw0rd-iii87");
                //NetworkCredential LoginInfo = new NetworkCredential(Convert.ToString(ConfigurationManager.AppSettings["goodnighttainan@gmail.com"]), Convert.ToString(ConfigurationManager.AppSettings["filgqxrexlhpuxso"]));
                NetworkCredential LoginInfo = new NetworkCredential("goodnighttainan@gmail.com", "filgqxrexlhpuxso");

                mySmtpClient.UseDefaultCredentials = true;

                mySmtpClient.EnableSsl = true; //gmail預設開啟驗證
                mySmtpClient.Credentials = LoginInfo;

                //add from, to mail addresses
                MailAddress from = new MailAddress("goodnighttainan@gmail.com", "好夜台南 Good Night Tainan", System.Text.Encoding.UTF8);
                MailAddress to = new MailAddress(mailAddress, receiver, System.Text.Encoding.UTF8);

                MailMessage myMail = new MailMessage(from, to);

                //add ReplyTo
                MailAddress replyTo = new MailAddress("goodnighttainan@gmail.com");
                myMail.ReplyToList.Add(replyTo);

                //set subject and encoding
                myMail.Subject = "【系統通知】好夜台南 Good Night Tainan 已收到您的意見回饋。";
                myMail.SubjectEncoding = System.Text.Encoding.UTF8;

                //set body message and encoding

                myMail.Body = "<h3>HI, " + receiver + ",</h3><p>    已收到您的意見回饋，感謝您使用【好夜台南 Good Night Tainan】，您寶貴的意見是我們進步的動力。</p><div align =\"center\" border=\"2px solid black\"> 意見回饋內容: " + reviewContent + "</div><div> -------------此為系統自動發送，請勿直接回覆。為了確保能收到來自【好夜台南 Good Night Tainan】的信件，請將goodnighttainan@gmail.com加入您的通訊錄-------------</div>";
                myMail.BodyEncoding = System.Text.Encoding.UTF8;

                //text or html;
                myMail.IsBodyHtml = true;
                
                mySmtpClient.Send(myMail);

            }
            catch (SmtpException ex)
            {
                throw new ApplicationException("smtpException has occured: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}