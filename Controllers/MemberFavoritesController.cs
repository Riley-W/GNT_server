using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using GNT_server.Models;

namespace GNT_server.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/MemberFavorites")]
    public class MemberFavoritesController : ApiController
    {
        private projectDBEntities db = new projectDBEntities();

        // GET: api/MemberFavorites
        /// <summary>
        /// 查詢所有最愛(後台)
        /// </summary>
        /// <returns></returns>
        [Route("")]
        public IQueryable<MemberFavorite> GetMemberFavorite()
        {
            return db.MemberFavorite;
        }

        // GET: api/MemberFavorites/5
        /// <summary>
        /// 查詢會員最愛(前台)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(MemberFavorite))]
        [Route("{id:int}")]
        public IHttpActionResult GetMemberFavorite(int id,int shop)
        {
            MemberFavorite memberFavorite = db.MemberFavorite.Find(id,shop);
            if (memberFavorite == null)
            {
                return NotFound();
            }

            return Ok(memberFavorite);
        }

        // PUT: api/MemberFavorites/5
        /// <summary>
        /// 修改我的最愛(前台)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="memberFavorite"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 新增我的最愛(前台)
        /// </summary>
        /// <param name="memberFavorite"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 刪除我的最愛(後台)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="shopid"></param>
        /// <returns></returns>
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