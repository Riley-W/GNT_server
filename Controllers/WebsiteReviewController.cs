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
        public IHttpActionResult GetWebsiteReview()
        {   //QueryAll
            var result = from w in db.WebsiteReview
                         orderby w.ReviewDate descending
                         select w;

            return Ok(WebsiteReviewTransfer.TransfertoDate(result));

        }

        /// <summary>
        /// 意見回饋類別查詢(後台) 
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        [Route("type/{type}")]
        public IHttpActionResult GetWebsiteReviewByType(string Type)
        {   //QueryByType: 推薦店家 系統回饋 店家資訊更新 其他
            var result = from w in db.WebsiteReview
                         where w.Type == Type
                         orderby w.ReviewDate descending
                         select w;

            return Ok(WebsiteReviewTransfer.TransfertoDate(result));
        }

        /// <summary>
        /// 意見回饋狀態查詢(後台)
        /// </summary>
        /// <param name="Status"></param>
        /// <returns></returns>
        [Route("status/{status}")]
        public IHttpActionResult GetWebsiteReviewByStatus(string Status)
        {   //QueryByStatus: 已處理 處理中 未處理
            var result = from w in db.WebsiteReview
                         where w.Status == Status
                         orderby w.ReviewDate descending
                         select w;

            return Ok(WebsiteReviewTransfer.TransfertoDate(result));
        }

        /// <summary>
        /// 意見回饋關鍵字查詢(後台)
        /// </summary>
        /// <param name="Keywords"></param>
        /// <returns></returns>
        [Route("keywords/{keywords}")]
        public IHttpActionResult GetWebsiteReviewByKeywords(string Keywords)
        {   //QueryByKeywords: 

            var result = from w in db.WebsiteReview
                         where (w.ReviewDate.ToString().Contains(Keywords) || w.Type.Contains(Keywords) ||
                         w.RContent.Contains(Keywords) || w.Status.Contains(Keywords) || w.Remark.Contains(Keywords))
                         orderby w.ReviewDate descending
                         select w;

            return Ok(WebsiteReviewTransfer.TransfertoDate(result));

        }

        /// <summary>
        /// 意見回饋ID查詢(後台)
        /// </summary>
        /// <param name="ReviewID"></param>
        /// <returns></returns>
        [Route("{ReviewID}")]
        [HttpGet]
        public IHttpActionResult GetWebsiteReviewByID(int ReviewID)
        {   //QueryByID
            var GetReviewByID = db.WebsiteReview.Where(p => p.ReviewID == ReviewID);
            var GetReviewByIDCount = db.WebsiteReview.Where(p => p.ReviewID == ReviewID).Count();
            if (GetReviewByIDCount < 1)
            {
                //throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NoContent) { Content = new StringContent("找無此ID。") });
                return Ok(WebsiteReviewTransfer.TransfertoDate(GetReviewByID));


            }
            return Ok(WebsiteReviewTransfer.TransfertoDate(GetReviewByID));

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
            else
            {
                WebsiteReview.ReviewDate = DateTime.Now;
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

            //return Ok(WebsiteReview); 
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

                //insert logo
                string Server_MapPath = System.Web.HttpContext.Current.Server.MapPath("/");
                Attachment logo = new Attachment(Server_MapPath + "/fonts/好夜台南_v3_01.jpeg");
                logo.ContentId = "logo.jpeg";
                myMail.Attachments.Add(logo);

                //set body message and encoding
                myMail.Body = "<div style=\"background-color: #E1E1E1;font-family: Microsoft JhengHei;text-align: center; padding: 70px 20px\"><a href=\"https://riley-w.github.io/goodNightTainan_front/\"><img src=\"cid:logo.jpeg\" width=\"300px\";></a><div style=\" padding:5% 10% ; margin: 5px; border: 2px transparent solid; text-align: center;font-family: Microsoft JhengHei; background-color: white\" >" + "<h3>HI, " + receiver + ",</h3><p style=\"font-size: 14px\">    意見回饋已收到，感謝您使用【好夜台南 Good Night Tainan】，您寶貴的意見是我們進步的動力。 </p><div style=\"border: 1px #CCC solid;background-color: #F5F5F5; border-radius: 3px;  margin: 3%;padding: 3%; text-align: left; font-size: 14px;line-height:200%\" width=\"600px\"> △ 回饋時間: " + receiveDate + "<br>△ 回饋內容: " + reviewContent + "<div style=\"text-align: center\"></div></div></div><div style=\"line-height:15px ; color: grey; padding: 20px\"> **此為系統自動發送，請勿直接回覆。為了確保能收到來自【好夜台南 Good Night Tainan】的信件，請將goodnighttainan@gmail.com加入您的通訊錄**</div></div>";

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

                //insert logo
                //LinkedResource logo = new LinkedResource(@"C:\Users\YU\Desktop\全端養成班\期末專題\Git\Merge_3_add jsonignore\GNT_server\fonts\好夜台南_v3_01.jpeg");
                string Server_MapPath = System.Web.HttpContext.Current.Server.MapPath("/");
                Attachment logo = new Attachment(Server_MapPath + "/fonts/好夜台南_v3_01.jpeg");
                logo.ContentId = "logo.jpeg";
                myMail.Attachments.Add(logo);
                //logo.ContentType = new ContentType(MediaTypeNames.Image.Jpeg);


                //set body message and encoding
                myMail.Body = "<div style=\"background-color: #E1E1E1;font-family: Microsoft JhengHei;text-align: center; padding: 70px 20px\"><a href=\"https://riley-w.github.io/goodNightTainan_front/\"><img src=\"cid:logo.jpeg\" width=\"300px\";></a><div style=\" padding:5% 10% ; margin: 5px; border: 2px transparent solid; text-align: center;font-family: Microsoft JhengHei; background-color: white\" >" + "<h3>HI, " + receiver + ",</h3><p style=\"font-size: 14px\">    意見回饋已收到，感謝您使用【好夜台南 Good Night Tainan】，您寶貴的意見是我們進步的動力。 </p><div style=\"border: 1px #CCC solid;background-color: #F5F5F5; border-radius: 3px;  margin: 3%;padding: 3%; text-align: left; font-size: 14px;line-height:200%\" width=\"600px\"> △ 回饋時間: " + receiveDate + "<br>△ 回饋內容: " + reviewContent + "<hr>△ 系統回覆: " + Reply + "<div style=\"text-align: center\"></div></div></div><div style=\"line-height:15px ; color: grey; padding: 20px\"> **此為系統自動發送，請勿直接回覆。為了確保能收到來自【好夜台南 Good Night Tainan】的信件，請將goodnighttainan@gmail.com加入您的通訊錄**</div></div>"; 

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
