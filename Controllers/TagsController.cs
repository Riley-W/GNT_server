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
    public class TagsController : ApiController
    {
        private projectDBEntities db = new projectDBEntities();

        /// <summary>
        /// 查詢所有Tags
        /// </summary>
        /// <returns></returns>
        // GET: api/Tags
        [Route("api/Tag/")]
        public IQueryable<Tag> GetTag()
        {
            return db.Tag;
        }

        //// GET: api/Tags/5
        //[ResponseType(typeof(Tag))]
        //public IHttpActionResult GetTag(int id)
        //{
        //    Tag tag = db.Tag.Find(id);
        //    if (tag == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(tag);
        //}

        //// PUT: api/Tags/5
        //[ResponseType(typeof(void))]
        //public IHttpActionResult PutTag(int id, Tag tag)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != tag.TagID)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(tag).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!TagExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        //// POST: api/Tags
        //[ResponseType(typeof(Tag))]
        //public IHttpActionResult PostTag(Tag tag)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.Tag.Add(tag);
        //    db.SaveChanges();

        //    return CreatedAtRoute("DefaultApi", new { id = tag.TagID }, tag);
        //}

        //// DELETE: api/Tags/5
        //[ResponseType(typeof(Tag))]
        //public IHttpActionResult DeleteTag(int id)
        //{
        //    Tag tag = db.Tag.Find(id);
        //    if (tag == null)
        //    {
        //        return NotFound();
        //    }

        //    db.Tag.Remove(tag);
        //    db.SaveChanges();

        //    return Ok(tag);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TagExists(int id)
        {
            return db.Tag.Count(e => e.ID == id) > 0;
        }
    }
}