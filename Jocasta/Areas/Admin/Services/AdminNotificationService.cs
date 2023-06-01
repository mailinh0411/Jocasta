using Dapper;
using Jocasta.Models;
using Jocasta.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Jocasta.Areas.Admin.Services
{
    public class AdminNotificationService : BaseService
    {
        public AdminNotificationService() : base() { }  
        public AdminNotificationService(IDbConnection db) : base(db) { }    
        public void InsertNotification(Notification model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[notification] ([NotificationId],[Content],[Link],[UserId],[CreateTime],[Title],[IsRead]) VALUES (@NotificationId,@Content,@Link,@UserId,@CreateTime,@Title,@IsRead)";
            int count = this._connection.Execute(query, model, transaction);
            if (count <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
    }
}