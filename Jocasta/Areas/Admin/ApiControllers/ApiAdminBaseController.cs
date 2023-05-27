using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Jocasta.Filters;
using Jocasta.Models;

namespace Jocasta.Areas.Admin.ApiControllers
{
    //[ApiAdminTokenRequire]
    public class ApiBaseAdminController : ApiController
    {       
        public JsonResult Success(object data = null, string message = null)
        {
            return new JsonResult { status = JsonResult.Status.SUCCESS, data = data, message = message };
        }

        public JsonResult Error(string message = JsonResult.Message.ERROR_SYSTEM)
        {
            return new JsonResult { status = JsonResult.Status.ERROR, message = message };
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult Unauthorized()
        {
            return new JsonResult { status = JsonResult.Status.UNAUTHORIZED, data = null, message = JsonResult.Message.NO_PERMISSION };
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult Unauthenticated()
        {
            return new JsonResult { status = JsonResult.Status.UNAUTHENTICATED, data = null, message = JsonResult.Message.TOKEN_EXPIRED };
        }

    }

}