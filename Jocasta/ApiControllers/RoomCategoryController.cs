using Jocasta.Models;
using Jocasta.Providers;
using Jocasta.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Jocasta.ApiControllers
{
    public class RoomCategoryController : ApiBaseController
    {
        // GET: RoomCategory
        [HttpGet]
        public JsonResult GetListRoomCategory()
        {
            try
            {
                RoomCategoryService roomCategoryService = new RoomCategoryService();
                List<RoomCategory> list = roomCategoryService.GetListRoomCategory();
                return Success(list);
            }catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}