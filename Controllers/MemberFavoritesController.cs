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
        /// <param name="Memberid"></param>
        /// <returns></returns>
        [ResponseType(typeof(MemberFavorite))]
        [Route("{Memberid:int}")]       
        public IHttpActionResult GetMemberFavorite(int Memberid)
        {
            
            //MemberFavorite memberFavorite = db.MemberFavorite.Find(id);
            //if (memberFavorite == null)
            //{
            //    return NotFound();
            //}
            var query = db.MemberFavorite.Where(o => o.MemberID == Memberid);
            //var querys = db.MemberFavorite.Where(o => o.ShopID == shopid );

            return Ok(query);
        }



        // POST: api/MemberFavorites
        /// <summary>
        /// 新增我的最愛(前台) 請帶入memberid: shopid:
        /// 
        /// </summary>
        /// <param name="memberFavorite"></param>
        /// <returns></returns>
        [ResponseType(typeof(MemberFavorite))]
        [Route("")]
        public IHttpActionResult PostMemberFavorite(MemberFavorite memberFavorite)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            memberFavorite.Status = true;
            db.MemberFavorite.Add(memberFavorite);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (MemberFavoriteExists(memberFavorite.MemberID))
                {
                    return BadRequest("錯誤: 我的最愛已存在");
                }
                else
                {
                    throw;
                }
            }

            return Ok("我的最愛新增成功");
        }

        // DELETE: api/MemberFavorites/5
        /// <summary>
        /// 刪除我的最愛(後台)
        /// </summary>
        /// <param name="Memberid"></param>
        /// <param name="shopid"></param>
        /// <returns></returns>
        [Route("{Memberid:int}/{shopid:int}")]
        [ResponseType(typeof(MemberFavorite))]
        public IHttpActionResult DeleteMemberFavorite(int Memberid, int shopid)
        {
            MemberFavorite memberFavorite = db.MemberFavorite.Find(Memberid, shopid);
            if (memberFavorite == null)
            {
                return BadRequest("錯誤: 無該筆資料"); ;
            }

            var query = from a in db.MemberFavorite
                        where (a.MemberID == Memberid &
                                a.ShopID == shopid )
                        select a;
            db.MemberFavorite.RemoveRange(query);
            db.SaveChanges();

            return Ok("刪除成功");
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