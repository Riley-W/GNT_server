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
    [RoutePrefix("api/Routes")]
    public class RoutesController : ApiController
    {
        private projectDBEntities db = new projectDBEntities();

        // GET: api/Routes
        [Route("")]
        public IQueryable<Route> GetRoute()
        {
            return db.Route;
        }

        // GET: api/Routes/5
        [ResponseType(typeof(Route))]
        [Route("{id:int}")]
        public IHttpActionResult GetRoute(int id)
        {
            Route route = db.Route.Find(id);
            if (route == null)
            {
                return NotFound();
            }

            return Ok(route);
        }

        // PUT: api/Routes/5
        [ResponseType(typeof(void))]
        [Route("{id:int}")]
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

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Routes
        [ResponseType(typeof(Route))]
        [Route("")]
        public IHttpActionResult PostRoute(Route route)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Route.Add(route);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = route.RouteID }, route);
        }

        // DELETE: api/Routes/5
        [ResponseType(typeof(Route))]
        [Route("{id:int}")]
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