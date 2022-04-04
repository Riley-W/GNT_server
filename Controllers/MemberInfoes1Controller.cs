using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using System.Web.Mvc;
using GNT_server.Models;
using RouteAttribute = System.Web.Http.RouteAttribute;
using RoutePrefixAttribute = System.Web.Http.RoutePrefixAttribute;

namespace GNT_server.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/MemberInfoes1")]
    public class MemberInfoes1Controller : ApiController
    {
        private projectDBEntities db = new projectDBEntities();

        // GET: api/MemberInfoes1
        /// <summary>
        /// 查詢所有會員(後台)
        /// </summary>
        /// <returns></returns>
        [Route("Admin")]
        public IHttpActionResult GetMemberInfoAdmin()
        {
            var result = from m in db.MemberInfo
                         select m;
            if (result == null)
            {
                return Content(HttpStatusCode.NotFound, "查無此會員");
            }
            else
            {
                var newMemberInfo = MemberInfoTransfer.TransfertoDateForAdmin(result);
                return Ok(newMemberInfo);
            }
        }




        // GET: api/MemberInfoes1/5
        /// <summary>
        /// 查詢會員by MemberID(前台)
        /// </summary>
        /// <param name="Memberid"></param>
        /// <returns></returns>
        [ResponseType(typeof(MemberInfo))]
        [Route("{Memberid:int}")]
        public IHttpActionResult GetMemberInfo(int Memberid)
        {
            var result = from m in db.MemberInfo
                         where m.MemberID==Memberid
                         select m;
            if (result == null)
            {
                return Content(HttpStatusCode.NotFound, "找無此會員");

            }
            else
            {
                var newMemberInfo = MemberInfoTransfer.TransfertoDateForUser(result);
                return Ok(newMemberInfo);
            }
        }


        // GET:
        /// <summary>
        /// 查詢為"黑名單"的會員 (後台)
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(MemberInfo))]
        [Route("Admin/BlackList")]
        public IHttpActionResult GetMemberInfoInBlacklist()
        {
            var result = from m in db.MemberInfo
                         where m.BlackList==true
                         select m;            
            if (result == null)
            {
                return Content(HttpStatusCode.NotFound, "無黑名單會員");

            }
            else
            {
                var newMemberInfo = MemberInfoTransfer.TransfertoDateForAdmin(result);
                return Ok(newMemberInfo);
            }
        }

        // GET:
        /// <summary>
        /// 依"關鍵字"查詢會員 (後台)
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(MemberInfo))]
        [Route("Admin/Keywords/{Keywords}")]
   
        public IHttpActionResult GetMemberInfoByKeywords(string Keywords)
        {
            var result = from m in db.MemberInfo
                         where (m.Name.Contains(Keywords) 
                         || m.Phone.Contains(Keywords) 
                         || m.Address.Contains(Keywords) 
                         || m.Gender.Contains(Keywords) 
                         || m.BirthDate.ToString().Contains(Keywords)
                         ||m.Email.Contains(Keywords))
                         select m;
            
            if (result == null)
            {
                return Content(HttpStatusCode.NotFound, "找無含有{Keywords}會員");

            }
            else
            {
                var newMemberInfo = MemberInfoTransfer.TransfertoDateForAdmin(result);
                return Ok(newMemberInfo);
            }
            
        }



        // PUT: api/MemberInfoes1/5
        /// <summary>
        /// 修改會員資料(前台)
        /// </summary>
        /// <param name="Memberid"></param>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        [Route("{Memberid:int}")]
        public IHttpActionResult PutMemberInfo(int Memberid, MemberInfo memberInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (Memberid != memberInfo.MemberID)
            {
                return BadRequest();
            }

            db.Entry(memberInfo).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberInfoExists(Memberid))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("會員資料修改成功");
        }

        // PUT: api/MemberInfoes1/5
        /// <summary>
        /// 修改會員資料(後台) 需要帶入會員ID
        /// </summary>
        /// <param name="Memberid"></param>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        [Route("Admin/{Memberid:int}")]
        public IHttpActionResult PutMemberInfoAdmin(int Memberid, MemberInfo memberInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (Memberid != memberInfo.MemberID)
            {
                return BadRequest();
            }

            db.Entry(memberInfo).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberInfoExists(Memberid))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("會員資料修改成功");
        }

        // POST: api/MemberInfoes1
        /// <summary>
        /// 新增會員(前台)
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        [ResponseType(typeof(MemberInfo))]
        [Route("")]
        public IHttpActionResult PostMemberInfo(MemberInfo memberInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var exist = db.MemberInfo.Any(m => m.Phone == memberInfo.Phone);
            var exists = db.MemberInfo.Any(m => m.Account == memberInfo.Account);
            if (exists == true)
            {
                return BadRequest("帳號已被使用");
            }else if (exist == true)
            {
                return BadRequest("電話已被註冊");
            }
            

           



            DateTime date = DateTime.Now;
            memberInfo.RegisterDate = date;
            memberInfo.BlackList = false;
            
            


            db.MemberInfo.Add(memberInfo);
            db.SaveChanges();

            return Ok("會員新增成功");
        }

        // POST: api/MemberInfoes1
        /// <summary>
        /// 新增會員(後台) 暫時無須此功能
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        [ResponseType(typeof(MemberInfo))]
        [Route("Admin")]
        public IHttpActionResult PostMemberInfoAdmin(MemberInfo memberInfo)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var exist = db.MemberInfo.Any(m => m.Phone == memberInfo.Phone);
            var exists = db.MemberInfo.Any(m => m.Account == memberInfo.Account);
            if (exists == true)
            {
                return BadRequest("帳號已被使用");
            }
            else if (exist == true)
            {
                return BadRequest("電話已被註冊");
            }






            DateTime date = DateTime.Now;
            memberInfo.RegisterDate = date;
            memberInfo.BlackList = false;




            db.MemberInfo.Add(memberInfo);
            db.SaveChanges();

            return Ok("會員新增成功");
        }

        // DELETE: api/MemberInfoes1/5
        /// <summary>
        /// 刪除會員(後台)
        /// </summary>
        /// <param name="Memberid"></param>
        /// <returns></returns>
        [ResponseType(typeof(MemberInfo))]
        [Route("Admin/{Memberid:int}")]
        public IHttpActionResult DeleteMemberInfo(int Memberid)
        {
            MemberInfo memberInfo = db.MemberInfo.Find(Memberid);
            var query = db.Route.Where(o => o.MemberID == Memberid);
            db.Route.RemoveRange(query);
            db.SaveChanges();
            var query2 = db.WebsiteReview.Where(o => o.MemberID == Memberid);
            db.WebsiteReview.RemoveRange(query2);
            db.SaveChanges();

            if (memberInfo == null)
            {
                return Content(HttpStatusCode.NotFound, "找無此會員");
            }

            db.MemberInfo.Remove(memberInfo);
            db.SaveChanges();

            return Ok("已刪除");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MemberInfoExists(int id)
        {
            return db.MemberInfo.Count(e => e.MemberID == id) > 0;
        }
    }
}