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
    
    public class AdminInfoesController : ApiController
    {
        private projectDBEntities db = new projectDBEntities();

        // GET: api/AdminInfoes
        /// <summary>
        /// 查詢所有管理者(後台)
        /// </summary>
        /// <returns></returns>
        public IQueryable<AdminInfo> GetAdminInfo()
        {
            return db.AdminInfo;
        }

        // GET: api/AdminInfoes/5
        /// <summary>
        /// 查詢Admin by ID(後台)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(AdminInfo))]
        public IHttpActionResult GetAdminInfo(int id)
        {
            AdminInfo adminInfo = db.AdminInfo.Find(id);
            if (adminInfo == null)
            {
                return NotFound();
            }

            return Ok(adminInfo);
        }

        // PUT: api/AdminInfoes/5
        /// <summary>
        /// 修改Admin資料(後台)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="adminInfo"></param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAdminInfo(int id, AdminInfo adminInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != adminInfo.AdminID)
            {
                return BadRequest();
            }

            db.Entry(adminInfo).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdminInfoExists(id))
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

        // POST: api/AdminInfoes
        /// <summary>
        /// 新增Admin(後台)
        /// </summary>
        /// <param name="adminInfo"></param>
        /// <returns></returns>
        [ResponseType(typeof(AdminInfo))]
        public IHttpActionResult PostAdminInfo(AdminInfo adminInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.AdminInfo.Add(adminInfo);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = adminInfo.AdminID }, adminInfo);
        }

        // DELETE: api/AdminInfoes/5
        //[ResponseType(typeof(AdminInfo))]
        //public IHttpActionResult DeleteAdminInfo(int id)
        //{
        //    AdminInfo adminInfo = db.AdminInfo.Find(id);
        //    if (adminInfo == null)
        //    {
        //        return NotFound();
        //    }

        //    db.AdminInfo.Remove(adminInfo);
        //    db.SaveChanges();

        //    return Ok(adminInfo);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AdminInfoExists(int id)
        {
            return db.AdminInfo.Count(e => e.AdminID == id) > 0;
        }
    }
}