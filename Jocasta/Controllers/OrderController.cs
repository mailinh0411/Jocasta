using Jocasta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jocasta.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order
        [Route("check-now/{checkin?}/{checkout?}/{room?}")]
        public ActionResult CheckNow(RoomCategoryCheckNow model)
        {
            ViewBag.CheckIn = model.CheckIn;
            ViewBag.CheckOut = model.CheckOut;
            ViewBag.RoomCategory = model.RoomCategory;
            return View();
        }

        [Route("book-now")]
        public ActionResult BookNow()
        {
            return View();
        }

        [Route("information-user-book")]
        public ActionResult InfoUserBook()
        {
            return View();
        }

        [Route("success-order")]
        public ActionResult SuccessOrder()
        {
            return View();
        }



    }
}