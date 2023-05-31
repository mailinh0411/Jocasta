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
    public class AdminOrderService : BaseService
    {
        public AdminOrderService() : base() { } 
        public AdminOrderService(IDbConnection db) : base(db) { }

        #region Order
        public ListOrderUserModel GetListOrder(string keyword, string status, int page, int pageSize, IDbTransaction transaction = null)
        {
            string querySelect = "select o.*, u.Name as UserName";
            string queryCount = "select count(*)";
            string query = " from [order] o left join [user] u on o.UserId = u.UserId where 1=1";
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = "%" + keyword.Replace(" ", "%") + "%";
                query += " and (o.Code like @keyword or u.Email like @keyword or u.[Phone] like @keyword)";
            }

            if (!string.IsNullOrEmpty(status))
            {
                query += " and o.Status = @status";
            }

            ListOrderUserModel list = new ListOrderUserModel();
            int totalRow = this._connection.Query<int>(queryCount + query, new { keyword, status }, transaction).FirstOrDefault();

            if (totalRow > 0)
            {
                list.TotalPage = (int)Math.Ceiling((decimal)totalRow / (decimal)pageSize);
            }
            int skip = (page - 1) * pageSize;

            query += " order by o.CreateTime desc OFFSET " + skip + " ROWS FETCH NEXT " + pageSize + " ROWS ONLY";
            list.List = new List<OrderUserModel>();
            list.List = this._connection.Query<OrderUserModel>(querySelect + query, new { keyword, status }, transaction).ToList();
            return list;
        }

        public Order GetOrderById(string orderId, IDbTransaction transaction = null)
        {
            string query = "select * from [order] where OrderId = @orderId";
            return this._connection.Query<Order>(query, new { orderId }, transaction).FirstOrDefault();
        }

        public void UpdateStatusOrder(string orderId, string status, IDbTransaction transaction = null)
        {
            string query = "update [order] set Status = @status where OrderId = @orderId";
            int count = this._connection.Execute(query, new { orderId, status }, transaction);
            if (count <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
        #endregion

        #region OrderDetail

        #endregion
    }
}