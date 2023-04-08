using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jocasta.Areas.Admin.Controllers
{
    public class ServiceController : Controller
    {
        // GET: Admin/Service
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddService()
        {
            return View();
        }

        public ActionResult UpdateService(string id)
        {
            ViewBag.Id = id;
            return View();
        }
    }
}