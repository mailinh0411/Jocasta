using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jocasta.Areas.Admin.Controllers
{
    public class DayRoomController : Controller
    {
        // GET: Admin/DayRoom
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddDayRoom()
        {
            return View();
        }
    }
}