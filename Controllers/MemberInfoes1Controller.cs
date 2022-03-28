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
        public IQueryable<MemberInfo> GetMemberInfoAdmin()
        {
            return db.MemberInfo;
        }




        // GET: api/MemberInfoes1/5
        /// <summary>
        /// 查詢會員(後台)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(MemberInfo))]
        [Route("Admin/{id:int}")]
        public IHttpActionResult GetMemberInfo(int id)
        {
            MemberInfo memberInfo = db.MemberInfo.Find(id);
            if (memberInfo == null)
            {
                return NotFound();
            }

            return Ok(memberInfo);
        }

        // PUT: api/MemberInfoes1/5
        /// <summary>
        /// 修改會員資料(前台)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        [Route("{id:int}")]
        public IHttpActionResult PutMemberInfo(int id, MemberInfo memberInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != memberInfo.MemberID)
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
                if (!MemberInfoExists(id))
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
        /// 修改會員資料(後台)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        [Route("Admin/{id:int}")]
        public IHttpActionResult PutMemberInfoAdmin(int id, MemberInfo memberInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != memberInfo.MemberID)
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
                if (!MemberInfoExists(id))
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
        /// 新增會員(後台)
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
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(MemberInfo))]
        [Route("Admin/{id:int}")]
        public IHttpActionResult DeleteMemberInfo(int id)
        {
            MemberInfo memberInfo = db.MemberInfo.Find(id);
            var query = db.Route.Where(o => o.MemberID == id);
            db.Route.RemoveRange(query);
            db.SaveChanges();
            var query2 = db.WebsiteReview.Where(o => o.MemberID == id);
            db.WebsiteReview.RemoveRange(query2);
            db.SaveChanges();

            if (memberInfo == null)
            {
                return NotFound();
            }

            db.MemberInfo.Remove(memberInfo);
            db.SaveChanges();

            return Ok(memberInfo);
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