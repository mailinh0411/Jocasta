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
            string query = "INSERT INTO [dbo].[notification] ([NotificationId],[Content],[Link],[UserId],[CreateTime],[Title], [IsRead]) VALUES (@NotificationId,@Content,@Link,@UserId,@CreateTime,@Title,@IsRead)";
            int count = this._connection.Execute(query, model, transaction);
            if (count <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public List<Notification> GetListNotificationByUserId(string userId, IDbTransaction transaction = null)
        {
            string query = "select * from [dbo].[notification] where UserId = @userId order by CreateTime desc";
            return this._connection.Query<Notification>(query, new {userId}, transaction).ToList();
        }

        public int CountNotificationNoReadByUserId(string userId, IDbTransaction transaction = null)
        {
            string query = "select count(*) from [dbo].[notification] where UserId = @userId and IsRead = 0";
            return this._connection.Query<int>(query, new {userId}, transaction).FirstOrDefault();
        }
    }
}