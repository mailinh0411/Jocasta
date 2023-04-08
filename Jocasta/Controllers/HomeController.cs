using Jocasta.Models;
using Jocasta.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jocasta.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // danh sách loại phòng mới
            RoomCategoryService roomCategoryService = new RoomCategoryService();
            List<RoomCategory> lsRoomCategoryNew = roomCategoryService.GetListRoomCategoryForHomePage();
            ViewBag.ListRoomCategoryNew = lsRoomCategoryNew;

            // danh sách dịch vụ mới
            ServiceRoomService serviceRoomService = new ServiceRoomService();
            List<Service> lsServiceNew = serviceRoomService.GetListServiceForHomePage();
            ViewBag.ListServiceNew = lsServiceNew;

            return View();
        }

        [Route("dang-nhap")]
        public ActionResult Login()
        {
            return View();
        }

        [Route("dang-ky")]
        public ActionResult Register()
        {
            return View();
        }
    }
}
