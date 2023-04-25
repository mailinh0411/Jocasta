using Dapper;
using Jocasta.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace Jocasta.Services
{
    public class CartService : BaseService
    {
        public CartService() : base() { }
        public CartService(IDbConnection db) : base(db) { }

        #region Cart
        public Cart GetCartByUserId(string userId, IDbTransaction transaction = null)
        {
            string query = "select * from [cart] where UserId = @userId";
            return this._connection.Query<Cart>(query, new {userId}, transaction).FirstOrDefault();
        }

        public Cart GetCartByUserCheckInCheckOut(string userId, long checkIn, long checkOut, IDbTransaction transaction = null)
        {
            string query = "select * from [cart] where UserId = @userId and CheckIn = @checkIn and CheckOut = @checkOut";
            return this._connection.Query<Cart>(query, new {userId, checkIn, checkOut}, transaction).FirstOrDefault();  
        }

        public void InsertCart(Cart cart, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[cart] ([CartId],[UserId],[TotalQuantity],[TotalPrice],[CheckIn],[CheckOut]) " +
                "VALUES (@CartId,@UserId,@TotalQuantity,@TotalPrice,@CheckIn,@CheckOut)";
            int status = this._connection.Execute(query,cart, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void UpdateCart(Cart cart, IDbTransaction transaction = null)
        {
            string query = "update [dbo].[cart] set [TotalQuantity] = [TotalQuantity] + @TotalQuantity, [TotalPrice] = [TotalPrice] + @TotalPrice where [CartId] = @CartId";
            int status = this._connection.Execute(query, cart, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void DeleteCart(string CartId, IDbTransaction transaction = null)
        {
            string query = "delete from [dbo].[cart] where [CartId] = @CartId";
            int status = this._connection.Execute(query, new {CartId}, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
        #endregion

        #region CartDetail
        public List<CartDetail> GetListCartDetailByCart(string cartId, IDbTransaction transaction = null)
        {
            string query = "select * from [cart_detail] where CartId = @cartId";
            return this._connection.Query<CartDetail>(query, new { cartId }, transaction).ToList();
        }

        public List<CartDetailModel> GetListRoomBookedByCart(string cartId, IDbTransaction transaction = null)
        {
            string query = "select cd.*, rc.Name, rc.Image, rc.Price from [cart_detail] cd left join [room_category] rc on cd.RoomCategoryId = rc.RoomCategoryId where CartId = @cartId";
            return this._connection.Query<CartDetailModel>(query, new {cartId}, transaction).ToList();
        }

        public CartDetail GetRoomBookedByCartRoom(string cartId, string roomCategoryId, IDbTransaction transaction = null)
        {
            string query = "select * from [cart_detail] where CartId = @cartId and RoomCategoryId = @roomCategoryId";
            return this._connection.Query<CartDetail>(query, new { cartId, roomCategoryId }, transaction).FirstOrDefault();
        }

        public void InsertCartDetail (CartDetail cartDetail, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[cart_detail] ([CartDetailId],[CartId],[RoomCategoryId],[Quantity])" +
                " VALUES (@CartDetailId,@CartId,@RoomCategoryId,@Quantity)";
            int status = this._connection.Execute(query, cartDetail, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void UpdateQuantityCartDetail (CartDetail cartDetail, IDbTransaction transaction = null)
        {
            string query = "update [cart_detail] set [Quantity] = @Quantity where [CartDetailId] = @CartDetailId";
            int status = this._connection.Execute(query, cartDetail, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void DeleteCartDetail (string id, IDbTransaction transaction = null)
        {
            string query = "delete from [cart_detail] where [CartDetailId] = @id";
            int status = this._connection.Execute(query, new {id}, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void DeleteCartDetailByCart(string CartId, IDbTransaction transaction = null)
        {
            string query = "delete from [cart_detail] where [CartId] = @CartId";
            int status = this._connection.Execute(query, new { CartId }, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
        #endregion
    }
}