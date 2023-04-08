using Jocasta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Jocasta.ApiControllers
{
    public class ApiBaseController : ApiController
    {
        public JsonResult Success(object data = null, string message = null)
        {
            return new JsonResult { status = JsonResult.Status.SUCCESS, data = data, message = message };
        }

        public JsonResult Error(string message = JsonResult.Message.ERROR_SYSTEM)
        {
            return new JsonResult { status = JsonResult.Status.ERROR, message = message };
        }

        public JsonResult Unauthorized()
        {
            return new JsonResult { status = JsonResult.Status.UNAUTHORIZED, message = JsonResult.Message.TOKEN_EXPIRED };
        }

    }
}