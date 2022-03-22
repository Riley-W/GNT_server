using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class MemberInfoesController : ApiController
    {
        private projectDBEntities db = new projectDBEntities();

        // GET: api/MemberInfoes
        public IQueryable<MemberInfo> GetMemberInfo()
        {
            return db.MemberInfo;
        }

        // GET: api/MemberInfoes/5
        [ResponseType(typeof(MemberInfo))]
        public IHttpActionResult GetMemberInfo(int id,MemberFavorite hihi)  
        {
            
            MemberInfo memberInfo = db.MemberInfo.Find(id);
            if (memberInfo == null)
            {
                return NotFound();
            }

            return Ok(memberInfo);
        }

        // PUT: api/MemberInfoes/5
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

        // POST: api/MemberInfoes
        [ResponseType(typeof(MemberInfo))]
        public IHttpActionResult PostMemberInfo(MemberInfo memberInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.MemberInfo.Add(memberInfo);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = memberInfo.MemberID }, memberInfo);
        }

        // DELETE: api/MemberInfoes/5
        [ResponseType(typeof(MemberInfo))]
        public IHttpActionResult DeleteMemberInfo(int id)
        {
            MemberInfo memberInfo = db.MemberInfo.Find(id);
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