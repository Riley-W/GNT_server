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
using LinqKit;

namespace GNT_server.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/Routes")]
    public class RoutesController : ApiController
    {
        private projectDBEntities db = new projectDBEntities();

        // GET: api/Routes
        /// <summary>
        /// 查詢所有會員的行程(後台)
        /// </summary>
        /// <returns></returns>
        [Route("Admin")]
        public List<object> GetRoute()
        {
            var route = from r in db.Route
                        select r;

            RouteTransfer aa = new RouteTransfer();

            return (RouteTransfer.changetime(route));
        }
        // GET: api/Routes/5
        /// <summary>
        /// 查詢單個會員的行程(前台)
        /// </summary>
        /// <param name="RouteID"></param>
        /// <returns></returns>
        [ResponseType(typeof(Route))]
        [Route("RouteID/{RouteID:int}")]

        public IHttpActionResult GetRouteByRouteID(int RouteID)
        {

            var result = db.Route.Where(o => o.RouteID == RouteID);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(RouteTransfer.changetime(result));

        }

        // GET: api/Routes/5
        /// <summary>
        /// 查詢單個會員的行程(前台)
        /// </summary>
        /// <param name="memberID"></param>
        /// <returns></returns>
        [ResponseType(typeof(Route))]
        [Route("{memberID:int}")]
        
        public IHttpActionResult GetRouteByMemberID(int memberID)
        {
            
            var result = db.Route.Where(o=>o.MemberID==memberID);
            
            if (result == null)
            {
                return NotFound();
            }

            return Ok(RouteTransfer.changetime(result));
            
        }



        // PUT: api/Routes/5
        /// <summary>
        /// 修改行程(前台)route裡面請必須輸入"RouteID" 即可更改Dest1~8或title
        /// </summary>
        /// <param name="Routeid"></param>
        /// <param name="route"></param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        [Route("{Routeid:int}")]
        public IHttpActionResult PutRoute(int Routeid ,Route route)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (Routeid != route.RouteID)
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
                if (!RouteExists(Routeid))
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

       


        // POST: api/Routes
        /// <summary>
        /// 新增行程(前台) 需帶入"memberID","Title","Dest"
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        [ResponseType(typeof(Route))]
        [Route("")]
        public IHttpActionResult PostRoute(Route route)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            DateTime date = DateTime.Now;
            route.AddDate = date;

            db.Route.Add(route);
            db.SaveChanges();

            return Ok("行程新增成功");
        }



        // DELETE: api/Routes/5
        /// <summary>
        /// 刪除會員自己其中一筆的行程(前台)
        /// </summary>
        /// <param name="Routeid"></param>
        /// <returns></returns>
        [ResponseType(typeof(Route))]
        [Route("{Routeid:int}")]
        public IHttpActionResult DeleteRoute(int Routeid)
        {
            Route route = db.Route.Find(Routeid);
            if (route == null)
            {
                return Content(HttpStatusCode.NotFound, "找無此行程");
            }

            db.Route.Remove(route);
            db.SaveChanges();

            return Ok("刪除成功");
        }

        // DELETE: api/Routes/5
        /// <summary>
        /// 刪除會員全部的行程(前台)
        /// </summary>
        /// <param name="Memberid"></param>
        /// <returns></returns>
        [ResponseType(typeof(Route))]
        [Route("deleteAll/{Memberid:int}")]
        public IHttpActionResult DeleteRouteAll(int Memberid)
        {
            var selectall = db.Route.Where(r => r.MemberID == Memberid);

            if (selectall == null)
            {
                return Content(HttpStatusCode.NotFound, "無行程可以刪除");
            }

            db.Route.RemoveRange(selectall);
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

        private bool RouteExists(int id)
        {
            return db.Route.Count(e => e.RouteID == id) > 0;
        }

        
    }
}