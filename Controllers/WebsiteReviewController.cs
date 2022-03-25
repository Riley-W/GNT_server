using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
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
        /// <summary>
        /// 查詢所有意見回饋(後台)
        /// </summary>
        /// <returns></returns>
        [Route("")]
        public IQueryable<WebsiteReview> GetWebsiteReview()
        {   //QueryAll
            return db.WebsiteReview;

        }

        /// <summary>
        /// 意見回饋類別查詢(後台) 
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        [Route("type/{type}")]
        public IQueryable<WebsiteReview> GetWebsiteReviewByType(string Type)
        {   //QueryByType: 推薦店家 系統回饋 店家資訊更新 其他
            return db.WebsiteReview.Where(p => p.Type == Type);
        }

        /// <summary>
        /// 意見回饋狀態查詢(後台)
        /// </summary>
        /// <param name="Status"></param>
        /// <returns></returns>
        [Route("status/{status}")]
        public IQueryable<WebsiteReview> GetWebsiteReviewByStatus(string Status)
        {   //QueryByStatus: 已處理 處理中 未處理
            return db.WebsiteReview.Where(p => p.Status == Status);
        }

        /// <summary>
        /// 意見回饋關鍵字查詢(後台)
        /// </summary>
        /// <param name="Keywords"></param>
        /// <returns></returns>
        [Route("keywords/{keywords}")]
        public IQueryable<WebsiteReview> GetWebsiteReviewByKeywords(string Keywords)
        {   //QueryByKeywords: 

            return db.WebsiteReview.Where(p => p.ReviewDate.ToString().Contains(Keywords) || p.Type.Contains(Keywords) || p.RContent.Contains(Keywords) || p.Status.Contains(Keywords) || p.Remark.Contains(Keywords));

        }

        /// <summary>
        /// 意見回饋ID查詢(後台)
        /// </summary>
        /// <param name="ReviewID"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 新增意見回饋(前台)
        /// </summary>
        /// <param name="WebsiteReview"></param>
        /// <returns></returns>
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
                var mailAddress = db.MemberInfo.Find(WebsiteReview.MemberID).Email.ToString();
                var receiver = db.MemberInfo.Find(WebsiteReview.MemberID).Name.ToString();
                DateTime receiveDateTime = (DateTime)WebsiteReview.ReviewDate;
                var receiveDate = receiveDateTime.ToString("yyyy/MM/dd");
                SendOKEmail(mailAddress, receiver, WebsiteReview.RContent, receiveDate);


                //return CreatedAtRoute("DefaultApi", new { MemberID = WebsiteReview.MemberID, ReviewDate = WebsiteReview.ReviewDate, Type = WebsiteReview.Type, RContent = WebsiteReview.RContent, Status = WebsiteReview.Status}, WebsiteReview);
                return Ok("資料新增完成。");
            }


        }

        //DELETE: 
        /// <summary>
        /// 刪除意見回饋(後台)
        /// </summary>
        /// <param name="ReviewID"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 編輯意見回饋(後台)
        /// </summary>
        /// <param name="ReviewID"></param>
        /// <param name="WebsiteReview"></param>
        /// <returns></returns>
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

                if (WebsiteReview.Status == "已處理")
                {
                    var mailAddress = db.MemberInfo.Find(WebsiteReview.MemberID).Email.ToString();
                    var receiver = db.MemberInfo.Find(WebsiteReview.MemberID).Name.ToString();

                    if (WebsiteReview.Remark == null)
                    {
                        return BadRequest("系統回覆不能為空。");
                    }
                    var Reply = WebsiteReview.Remark.ToString();
                    DateTime receiveDateTime = (DateTime)WebsiteReview.ReviewDate;
                    var receiveDate = receiveDateTime.ToString("yyyy/MM/dd");
                    SendEditEmail(mailAddress, receiver, WebsiteReview.RContent, Reply, receiveDate);
                }



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

        private void SendOKEmail(string mailAddress, string receiver, string reviewContent, string receiveDate)
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
                myMail.Subject = "【好夜台南 Good Night Tainan】 已收到意見回饋";
                myMail.SubjectEncoding = System.Text.Encoding.UTF8;

                //set body message and encoding

                myMail.Body = "<div style=\" padding: 24px; margin: auto 50px; border: 2px transparent solid; text-align: center  \">" + "<h2>HI, " + receiver + ",</h2><p>    意見回饋已收到，感謝您使用【好夜台南 Good Night Tainan】，您寶貴的意見是我們進步的動力。 </p><div style=\"border: 4px #E08E45 solid; background-color:#F8F4A6; border-radius: 12px; margin: 24px 200px ; padding: 24px; text-align: left; font-size: 16px; color : black\"> △回饋時間:" + receiveDate + "<br>△回饋內容: " + reviewContent + "<br><br><br><br>瀏覽<a href=\"https://riley-w.github.io/goodNightTainan_front/\">【好夜台南 Good Night Tainan】</a></div></div><div style=\"line-height:15px ; color: grey; margin-top: 20px\"> -----------------------------此為系統自動發送，請勿直接回覆。為了確保能收到來自【好夜台南 Good Night Tainan】的信件，請將goodnighttainan@gmail.com加入您的通訊錄-----------------------------</div>";

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

        private void SendEditEmail(string mailAddress, string receiver, string reviewContent, string Reply, string receiveDate)
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
                myMail.Subject = "【好夜台南 Good Night Tainan】 已回覆意見回饋";
                myMail.SubjectEncoding = System.Text.Encoding.UTF8;

                //set body message and encoding

                myMail.Body = "<div style=\" padding: 24px; margin: auto 50px; border: 2px transparent solid; text-align: center  \">" + "<h2>HI, " + receiver + ",</h2><p>    感謝您來信【好夜台南 Good Night Tainan】，您的意見回饋已處理： </p><div style=\"border: 4px #E08E45 solid; background-color:#F8F4A6; border-radius: 12px; margin: 24px 200px ; padding: 24px; text-align: left; font-size: 16px; color : black\"> △回饋時間:" + receiveDate + "<br>△回饋內容: " + reviewContent + "<br>---------------------------------------------------------------------------------------------------<br>△系統回覆: " + Reply + "<br><br><br><br>瀏覽<a href=\"https://riley-w.github.io/goodNightTainan_front/\">【好夜台南 Good Night Tainan】</a></div></div><div style=\"line-height:15px ; color: grey; margin-top: 20px\"> -----------------------------此為系統自動發送，請勿直接回覆。為了確保能收到來自【好夜台南 Good Night Tainan】的信件，請將goodnighttainan@gmail.com加入您的通訊錄-----------------------------</div>";
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