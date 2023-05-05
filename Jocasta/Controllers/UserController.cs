using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jocasta.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        
        [Route("user/user-info")]
        public ActionResult UserInfo()
        {
            return View();
        }


        [Route("user/change-password")]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [Route("user/list-order")]
        public ActionResult ListOrder()
        {
            return View();
        }

        [Route("user/list-order/{id}")]
        public ActionResult OrderDetail(string id)
        {
            ViewBag.Id = id;
            return View();
        }
    }
}