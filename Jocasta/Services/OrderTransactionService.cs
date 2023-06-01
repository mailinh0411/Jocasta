using Dapper;
using Jocasta.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Jocasta.Services
{
    public class OrderTransactionService : BaseService
    {
        public OrderTransactionService() : base() { }
        public OrderTransactionService(IDbConnection db) : base(db) { }
        public void InsertOrderTransaction(OrderTransaction model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[order_transaction] ([OrderTransactionId],[OrderId],[UserAdminId],[Status],[Content],[CreateTime]) VALUES (@OrderTransactionId,@OrderId,@UserAdminId,@Status,@Content,@CreateTime)";
            int count = this._connection.Execute(query, model, transaction);
            if (count <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
    }
}