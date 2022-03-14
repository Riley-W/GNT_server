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
    public class WebsiteReviewsController : ApiController
    {
        private projectDBEntities1 db = new projectDBEntities1();

        // GET: api/WebsiteReviews
        public IQueryable<WebsiteReview> GetWebsiteReview()
        {
            return db.WebsiteReview;
        }

        // GET: api/WebsiteReviews/5
        [ResponseType(typeof(WebsiteReview))]
        public IHttpActionResult GetWebsiteReview(int id)
        {
            WebsiteReview websiteReview = db.WebsiteReview.Find(id);
            if (websiteReview == null)
            {
                return NotFound();
            }

            return Ok(websiteReview);
        }

        // PUT: api/WebsiteReviews/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutWebsiteReview(int id, WebsiteReview websiteReview)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != websiteReview.ReviewID)
            {
                return BadRequest();
            }

            db.Entry(websiteReview).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WebsiteReviewExists(id))
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

        // POST: api/WebsiteReviews
        [ResponseType(typeof(WebsiteReview))]
        public IHttpActionResult PostWebsiteReview(WebsiteReview websiteReview)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.WebsiteReview.Add(websiteReview);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = websiteReview.ReviewID }, websiteReview);
        }

        // DELETE: api/WebsiteReviews/5
        [ResponseType(typeof(WebsiteReview))]
        public IHttpActionResult DeleteWebsiteReview(int id)
        {
            WebsiteReview websiteReview = db.WebsiteReview.Find(id);
            if (websiteReview == null)
            {
                return NotFound();
            }

            db.WebsiteReview.Remove(websiteReview);
            db.SaveChanges();

            return Ok(websiteReview);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool WebsiteReviewExists(int id)
        {
            return db.WebsiteReview.Count(e => e.ReviewID == id) > 0;
        }
    }
}