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
            string query = "INSERT INTO [dbo].[order] ([OrderId],[UserId],[TotalPrice],[Status],[CreateTime],[CheckIn],[CheckOut],[Code],[Email],[Phone],[Name],[RequestContent]) " +
                "VALUES (@OrderId,@UserId,@TotalPrice,@Status,@CreateTime,@CheckIn,@CheckOut,@Code,@Email,@Phone,@Name,@RequestContent)";
            int status = this._connection.Execute(query, order, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public List<Order> GetListOrderByUserId(string userId, IDbTransaction transaction = null)
        {
            string query = "select * from [order] where UserId = @userId order by CreateTime desc";
            return this._connection.Query<Order>(query, new {userId}, transaction).ToList();
        }

        public Order GetOrderById(string id, IDbTransaction transaction = null)
        {
            string query = "select * from [order] where OrderId = @id";
            return this._connection.Query<Order>(query, new { id }, transaction).FirstOrDefault();
        }

        public void UpdateStatusOrder(string orderId, string status, IDbTransaction transaction = null)
        {
            string query = "update [order] set [Status] = @status where OrderId = @orderId";
            int count = this._connection.Execute(query, new { orderId, status }, transaction);
            if (count <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void UpdateTotalPriceOrder(string orderId, decimal totalPrice, IDbTransaction transaction = null)
        {
            string query = "update [order] set [TotalPrice] = [TotalPrice] + @totalPrice where [OrderId] = @orderId";
            int count = this._connection.Execute(query, new {orderId, totalPrice}, transaction);
            if (count <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
        #endregion

        #region OrderDetail
        public void InsertOrderDetail(OrderDetail orderDetail, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[order_detail] ([OrderDetailId],[OrderId],[RoomCategoryId],[NumberOfRoom],[Price]) " +
                "VALUES (@OrderDetailId,@OrderId,@RoomCategoryId,@NumberOfRoom,@Price)";
            int status = this._connection.Execute(query, orderDetail, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public List<OrderDetail> GetListOrderDetailByOrderId(string orderId, IDbTransaction transaction = null)
        {
            string query = "select * from [order_detail] where OrderId = @orderId";
            return this._connection.Query<OrderDetail>(query, new { orderId }, transaction).ToList();
        }
        #endregion
    }
}