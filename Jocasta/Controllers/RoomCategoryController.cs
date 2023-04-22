using Jocasta.Models;
using Jocasta.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jocasta.Controllers
{
    public class RoomCategoryController : Controller
    {
        // GET: RoomCategory
        [Route("room-category/danh-sach/{page?}/{keyword?}")]
        public ActionResult Index(RoomCategorySearchModel searchModel)
        {
            RoomCategorySearchModel roomCategorySearch = new RoomCategorySearchModel();
            roomCategorySearch.CurrentPage = searchModel.CurrentPage;
            roomCategorySearch.Keyword = searchModel.Keyword;
            
            RoomCategoryService roomCategoryService = new RoomCategoryService();
            ListRoomCategoryView roomCategory = roomCategoryService.GetListRoomCategory(roomCategorySearch);
            ViewBag.ListRoomCategory = roomCategory;
            return View();
        }

        [Route("check-now/{checkin?}/{checkout?}/{room?}")]
        public ActionResult BookNow(RoomCategoryCheckNow model)
        {
            ViewBag.CheckIn = model.CheckIn;
            ViewBag.CheckOut = model.CheckOut;  
            ViewBag.RoomCategory = model.RoomCategory;
            return View();
        }

        [Route("book-now")]
        public ActionResult CheckNow()
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