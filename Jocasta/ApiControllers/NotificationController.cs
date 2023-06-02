using Jocasta.Areas.Admin.Services;
using Jocasta.Filters;
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
        [ApiTokenRequire]
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
        [ApiTokenRequire]
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

        [HttpGet]
        [ApiTokenRequire]
        public JsonResult UpdateNotificationRead(string id)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {                        
                        NotificationService notificationService = new NotificationService(connect);
                        Notification notification = notificationService.GetNotificationById(id, transaction);
                        notification.IsRead = true;
                        notificationService.UpdateNotificationRead(notification, transaction);
                        transaction.Commit();
                        return Success();
                    }
                }

            }
            catch (Exception ex)
            {
                return Error();
            }
        }

        [HttpGet]
        [ApiTokenRequire]
        public JsonResult DeleteNoficationById(string id)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        NotificationService notificationService = new NotificationService(connect);
                        Notification notification = notificationService.GetNotificationById(id, transaction);
                        if (notification == null) throw new Exception("Thông báo này không tồn tại.");

                        notificationService.DeleteNotificationById(id, transaction);
                        transaction.Commit();
                        return Success();
                    }
                }

            }
            catch (Exception ex)
            {
                return Error();
            }
        }

        [HttpGet]
        [ApiTokenRequire]
        public JsonResult DeleteAllNotification()
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        string token = Request.Headers.Authorization.ToString();
                        UserService userService = new UserService(connect);
                        User user = userService.GetUserByToken(token, transaction);
                        if (user == null) return Unauthorized();

                        NotificationService notificationService = new NotificationService(connect);

                        notificationService.DeleteAllNotificationByUserId(user.UserId, transaction);
                        transaction.Commit();
                        return Success();
                    }
                }

            }
            catch (Exception ex)
            {
                return Error();
            }
        }
    }
}