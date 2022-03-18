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
using GNT_server.Models;

namespace GNT_server.Controllers
{
    public class MemberFavoritesController : ApiController
    {
        private projectDBEntities2 db = new projectDBEntities2();

        // GET: api/MemberFavorites
        public IQueryable<MemberFavorite> GetMemberFavorite()
        {
            return db.MemberFavorite;
        }

        // GET: api/MemberFavorites/5
        [ResponseType(typeof(MemberFavorite))]
        public IHttpActionResult GetMemberFavorite(int id)
        {
            MemberFavorite memberFavorite = db.MemberFavorite.Find(id);
            if (memberFavorite == null)
            {
                return NotFound();
            }

            return Ok(memberFavorite);
        }

        // PUT: api/MemberFavorites/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutMemberFavorite(int id, MemberFavorite memberFavorite)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != memberFavorite.MemberID)
            {
                return BadRequest();
            }

            db.Entry(memberFavorite).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberFavoriteExists(id))
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

        // POST: api/MemberFavorites
        [ResponseType(typeof(MemberFavorite))]
        public IHttpActionResult PostMemberFavorite(MemberFavorite memberFavorite)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.MemberFavorite.Add(memberFavorite);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (MemberFavoriteExists(memberFavorite.MemberID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = memberFavorite.MemberID }, memberFavorite);
        }

        // DELETE: api/MemberFavorites/5
        [ResponseType(typeof(MemberFavorite))]
        public IHttpActionResult DeleteMemberFavorite(int id,int shopid)
        {
            MemberFavorite memberFavorite = db.MemberFavorite.Find(id,shopid);
            if (memberFavorite == null)
            {
                return NotFound();
            }

            db.MemberFavorite.Remove(memberFavorite);
            db.SaveChanges();

            return Ok(memberFavorite);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MemberFavoriteExists(int id)
        {
            return db.MemberFavorite.Count(e => e.MemberID == id) > 0;
        }
    }
}