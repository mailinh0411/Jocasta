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
    public class AdminOrderTransactionService : BaseService
    {
        public AdminOrderTransactionService() : base() { }
        public AdminOrderTransactionService(IDbConnection db) : base(db) { }

        public List<OrderTransaction> GetListOrderTransactionByOrderId(string orderId, IDbTransaction transaction = null)
        {
            string query = "select * from [order_transaction] where OrderId = @orderId";
            return this._connection.Query<OrderTransaction>(query, new { orderId }, transaction).ToList();
        }

        public void InsertOrderTransaction(OrderTransaction model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[order_transaction] ([OrderTransactionId],[OrderId],[UserAdminId],[Status],[Content],[CreateTime]) VALUES (@OrderTransactionId,@OrderId,@UserAdminId,@Status,@Content,@CreateTime)";
            int count = this._connection.Execute(query, model, transaction);
            if (count <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
    }
}