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
    [RoutePrefix("api/RoutesAdmin")]
    public class RoutesAdminController : ApiController
    {
        private projectDBEntities db = new projectDBEntities();

        // GET: api/RoutesAdmin
        /// <summary>
        /// 查詢所有會員的行程(後台)
        /// </summary>
        /// <returns></returns>
        [Route("Admin")]
        public IQueryable<Route> GetRoute()
        {
            return db.Route;
        }

        // GET: api/RoutesAdmin/5
        /// <summary>
        /// 查詢單個會員的行程(後台)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(Route))]
        [Route("Admin/{id:int}")]
        public IHttpActionResult GetRoute(int id)
        {
            Route route = db.Route.Find(id);
            if (route == null)
            {
                return NotFound();
            }

            return Ok(route);
        }

        // PUT: api/RoutesAdmin/5
        /// <summary>
        /// 修改行程(後台)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="route"></param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        [Route("Admin/{RouteID:int}")]
        public IHttpActionResult PutRoute(int id, Route route)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != route.RouteID)
            {
                return BadRequest();
            }

            db.Entry(route).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RouteExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("行程修改成功");
        }

        // POST: api/RoutesAdmin
        /// <summary>
        /// 新增行程(後台)
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        [ResponseType(typeof(Route))]
        [Route("Admin")]
        public IHttpActionResult PostRoute(Route route)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Route.Add(route);
            db.SaveChanges();

            return Ok("行程新增成功");
        }

        // DELETE: api/RoutesAdmin/5
        /// <summary>
        /// 刪除行程(後台)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(Route))]
        [Route("Admin/{RouteID:int}")]
        public IHttpActionResult DeleteRoute(int id)
        {
            Route route = db.Route.Find(id);
            if (route == null)
            {
                return NotFound();
            }

            db.Route.Remove(route);
            db.SaveChanges();

            return Ok(route);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RouteExists(int id)
        {
            return db.Route.Count(e => e.RouteID == id) > 0;
        }
    }
}