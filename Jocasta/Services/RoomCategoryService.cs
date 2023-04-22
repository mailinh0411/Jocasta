using Dapper;
using Jocasta.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Jocasta.Services
{
    public class RoomCategoryService : BaseService
    {
        public RoomCategoryService() : base() { }
        public RoomCategoryService(IDbConnection db) : base(db) { }

        public RoomCategory GetRoomCategoryById(string id, IDbTransaction transaction = null)
        {
            string query = "select * from [room_category] where RoomCategoryId = @id";
            return this._connection.Query<RoomCategory>(query, new {id}, transaction).FirstOrDefault();
        }
        public List<RoomCategory> GetListRoomCategoryForHomePage(IDbTransaction transaction = null)
        {
            string query = "select Top(6) * from [room_category] where Enable=1 ORDER BY CreateTime DESC";
            return this._connection.Query<RoomCategory>(query, null, transaction).ToList();
        }

        public ListRoomCategoryView GetListRoomCategory(RoomCategorySearchModel searchModel, IDbTransaction transaction = null)
        {
            string querySelect = "select *";
            string queryCount = "select count(*)";
            string query = " from [room_category] where 1=1";
            if (!string.IsNullOrEmpty(searchModel.Keyword))
            {
                searchModel.Keyword = "%" + searchModel.Keyword.Replace(" ", "%") + "%";
                query += " and (Name like @Keyword or [View] like @Keyword)";
            }

            ListRoomCategoryView list = new ListRoomCategoryView();
            int totalRow = this._connection.Query<int>(queryCount + query, searchModel, transaction).FirstOrDefault();

            if (totalRow > 0)
            {
                list.TotalPage = (int)Math.Ceiling((decimal)totalRow / (decimal)Constant.PAGE_SIZE);
            }
            if (searchModel.CurrentPage == 0)
            {
                searchModel.CurrentPage = 1;
            }
            int skip = (searchModel.CurrentPage - 1) * Constant.PAGE_SIZE;

            query += " order by CreateTime desc OFFSET " + skip + " ROWS FETCH NEXT " + Constant.PAGE_SIZE + " ROWS ONLY";
            list.List = new List<RoomCategory>();
            list.List = this._connection.Query<RoomCategory>(querySelect + query, searchModel, transaction).ToList();
            return list;
        }

        public List<RoomCategory> GetAllByKeyword(string id, IDbTransaction transaction = null)
        {
            string query = "select * from [room_category] where 1=1";
            if (!string.IsNullOrEmpty(id))
            {
                query += " and RoomCategoryId = @id";
            }
            return this._connection.Query<RoomCategory>(query, new {id }, transaction).ToList();    
        }

        public List<RoomCategory> GetListRoomCategory(IDbTransaction transaction = null)
        {
            string query = "select * from [room_category]";
            return this._connection.Query<RoomCategory>(query, transaction).ToList();
        }
    }
}