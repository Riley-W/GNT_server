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
    public class ShopInfoesController : ApiController
    {
        private projectDBEntities1 db = new projectDBEntities1();

        // GET: api/ShopInfoes
        public IQueryable<ShopInfo> GetShopInfo()
        {
            return db.ShopInfo;
        }

        // GET: api/ShopInfoes/5
        [ResponseType(typeof(ShopInfo))]
        public IHttpActionResult GetShopInfo(int id)
        {
            ShopInfo shopInfo = db.ShopInfo.Find(id);
            if (shopInfo == null)
            {
                return NotFound();
            }

            return Ok(shopInfo);
        }

        // PUT: api/ShopInfoes/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutShopInfo(int id, ShopInfo shopInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != shopInfo.ShopID)
            {
                return BadRequest();
            }

            db.Entry(shopInfo).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShopInfoExists(id))
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

        // POST: api/ShopInfoes
        [ResponseType(typeof(ShopInfo))]
        public IHttpActionResult PostShopInfo(ShopInfo shopInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ShopInfo.Add(shopInfo);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = shopInfo.ShopID }, shopInfo);
        }

        // DELETE: api/ShopInfoes/5
        [ResponseType(typeof(ShopInfo))]
        public IHttpActionResult DeleteShopInfo(int id)
        {
            ShopInfo shopInfo = db.ShopInfo.Find(id);
            if (shopInfo == null)
            {
                return NotFound();
            }

            db.ShopInfo.Remove(shopInfo);
            db.SaveChanges();

            return Ok(shopInfo);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ShopInfoExists(int id)
        {
            return db.ShopInfo.Count(e => e.ShopID == id) > 0;
        }
    }
}