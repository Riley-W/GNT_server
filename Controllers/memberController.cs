using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Cors;
using System.Web.Mvc;

namespace GNT_server.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class memberController : Controller
    {
        // GET: asd
        public ActionResult Index()
        {
            return View();
        }
    }
}