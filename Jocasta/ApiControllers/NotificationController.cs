using Jocasta.Areas.Admin.Services;
using Jocasta.Models;
using Jocasta.Providers;
using Jocasta.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.UI.WebControls;

namespace Jocasta.ApiControllers
{
    public class NotificationController : ApiBaseController
    {
        [HttpGet]
        public JsonResult GetListNotificationByUserId()
        {
            try
            {
                string token = Request.Headers.Authorization.ToString();
                UserService userService = new UserService();

                NotificationService notificationService = new NotificationService();

                User user = userService.GetUserByToken(token);
                if (user == null) return Unauthorized();

                return Success(notificationService.GetListNotificationByUserId(user.UserId));

            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetCountNotificationNoRead()
        {
            try
            {
                string token = Request.Headers.Authorization.ToString();
                UserService userService = new UserService();

                NotificationService notificationService = new NotificationService();

                User user = userService.GetUserByToken(token);
                if (user == null) return Unauthorized();

                return Success(notificationService.CountNotificationNoReadByUserId(user.UserId));

            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}