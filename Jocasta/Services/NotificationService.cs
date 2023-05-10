﻿using Dapper;
using Jocasta.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Jocasta.Services
{
    public class NotificationService : BaseService
    {
        public NotificationService() : base() { }
        public NotificationService(IDbConnection db) : base(db) { }

        public void InsertNotification(Notification model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[notification] ([NotificationId],[Content],[Link],[UserId],[CreateTime],[Title]) VALUES (@NotificationId,@Content,@Link,@UserId,@CreateTime,@Title)";
            int count = this._connection.Execute(query, model, transaction);
            if (count <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
    }
}