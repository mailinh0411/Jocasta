using Dapper;
using Jocasta.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Jocasta.Services
{
    public class OrderService : BaseService
    {
        public OrderService() : base() { }
        public OrderService(IDbConnection db) : base(db) { }

        #region Order
        public void InsertOrder(Order order, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[order] ([OrderId],[UserId],[TotalPrice],[Status],[CreateTime]) VALUES (@OrderId,@UserId,@TotalPrice,@Status,@CreateTime)";
            int status = this._connection.Execute(query, order, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
        #endregion

        #region OrderDetail
        public void InsertOrderDetail(OrderDetail orderDetail, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[order_detail] ([OrderDetailId],[OrderId],[RoomCategoryId],[NumberOfRoom],[CheckIn],[CheckOut]) " +
                "VALUES (@OrderDetailId,@OrderId,@RoomCategoryId,@NumberOfRoom,@CheckIn,@CheckOut)";
            int status = this._connection.Execute(query, orderDetail, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
        #endregion
    }
}