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

namespace GNT_server.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MemberInfoes1Controller : ApiController
    {
        private projectDBEntities db = new projectDBEntities();

        // GET: api/MemberInfoes1
        public IQueryable<MemberInfo> GetMemberInfo()
        {
            return db.MemberInfo;
        }

        // GET: api/MemberInfoes1/5
        [ResponseType(typeof(MemberInfo))]
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
        [ResponseType(typeof(void))]
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

            return StatusCode(HttpStatusCode.NoContent);
        }
       
        // POST: api/MemberInfoes1
        [ResponseType(typeof(MemberInfo))]
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

            return CreatedAtRoute("DefaultApi", new { id = memberInfo.MemberID }, memberInfo);
        }

        // DELETE: api/MemberInfoes1/5
        [ResponseType(typeof(MemberInfo))]
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