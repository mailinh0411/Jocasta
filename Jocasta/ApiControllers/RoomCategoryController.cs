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
                return Success();
            }catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}