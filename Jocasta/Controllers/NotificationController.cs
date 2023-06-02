using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jocasta.Controllers
{
    public class NotificationController : Controller
    {
        // GET: Notification
        [Route("notification")]
        public ActionResult Index()
        {
            return View();
        }
    }
}