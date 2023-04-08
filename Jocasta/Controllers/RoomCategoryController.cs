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
            roomCategorySearch.CurrentPage =1;
            roomCategorySearch.Keyword = searchModel.Keyword;
            
            RoomCategoryService roomCategoryService = new RoomCategoryService();
            ListRoomCategoryView roomCategory = roomCategoryService.GetListRoomCategory(roomCategorySearch);
            ViewBag.ListRoomCategory = roomCategory;
            return View();
        }
    }
}