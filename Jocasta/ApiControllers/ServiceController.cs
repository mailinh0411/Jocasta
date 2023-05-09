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
    public class ServiceController : ApiBaseController
    {
        [HttpGet]
        public JsonResult GetListService()
        {
            try
            {
                ServiceRoomService serviceRoomService = new ServiceRoomService();
                return Success(serviceRoomService.GetListService());
            }catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}