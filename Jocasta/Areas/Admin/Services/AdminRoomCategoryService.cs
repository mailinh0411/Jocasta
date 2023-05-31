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
    public class AdminRoomCategoryService : BaseService
    {
        public AdminRoomCategoryService() : base() { }
        public AdminRoomCategoryService(IDbConnection db) : base(db) { }

        #region RoomCategory
        public List<RoomCategory> GetRoomCategories(string keyword, IDbTransaction transaction = null)
        {
            string query = "select * from [room_category] where Enable=1";
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = "%" + keyword.Replace(" ", "%") + "%";
                query += " and SearchName like @keyword";
            }
            return this._connection.Query<RoomCategory>(query, new { keyword }, transaction).ToList();
        }
        public ListRoomCategoryView GetListRoomCategory(string keyword, bool enable, int page, int pageSize, IDbTransaction transaction = null)
        {
            string querySelect = "select *";
            string queryCount = "select count(*)";
            string query = " from [room_category] where Enable = @enable";
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = "%" + keyword.Replace(" ", "%") + "%";
                query += " and (SearchName like @keyword or [View] like @keyword)";
            }

            ListRoomCategoryView list = new ListRoomCategoryView();
            int totalRow = this._connection.Query<int>(queryCount + query, new { keyword, enable }, transaction).FirstOrDefault();

            if (totalRow > 0)
            {
                list.TotalPage = (int)Math.Ceiling((decimal)totalRow / (decimal)pageSize);
            }
            int skip = (page - 1) * pageSize;

            query += " order by CreateTime desc OFFSET " + skip + " ROWS FETCH NEXT " + pageSize + " ROWS ONLY";
            list.List = new List<RoomCategory>();
            list.List = this._connection.Query<RoomCategory>(querySelect + query, new { keyword, enable }, transaction).ToList();
            return list;
        }

        public RoomCategory GetRoomCategoryById(string id, IDbTransaction transaction = null)
        {
            string query = "select * from room_category where RoomCategoryId = @id";
            return this._connection.Query<RoomCategory>(query, new { id }, transaction).FirstOrDefault();
        }

        public bool InsertRoomCategory(RoomCategory model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[room_category] ([RoomCategoryId],[Name],[View],[Square],[NumberOfPeople],[SingleBed],[DoubleBed],[Price],[Description],[Enable],[CreateTime],[Image],[SearchName]) " +
                "VALUES(@RoomCategoryId, @Name, @View, @Square, @NumberOfPeople, @SingleBed, @DoubleBed, @Price, @Description, @Enable, @CreateTime, @Image, @SearchName)";
            int status = this._connection.Execute(query, model, transaction);
            return status > 0;
        }

        public bool UpdateRoomCategory(RoomCategory model, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[room_category] SET [Name] = @Name,[View] = @View,[Square] = @Square,[NumberOfPeople] = @NumberOfPeople,[SingleBed] = @SingleBed,[DoubleBed] = @DoubleBed,[Price] = @Price,[Description] = @Description,[Image]=@Image, [SearchName]=@SearchName WHERE [RoomCategoryId] = @RoomCategoryId";
            int status = this._connection.Execute(query, model, transaction);
            return status > 0;
        }

        public bool DeleteRoomCategory(string id, IDbTransaction transaction = null)
        {
            string query = "DELETE FROM [dbo].[room_category] WHERE RoomCategoryId = @id";
            int status = this._connection.Execute(query, new { id }, transaction);
            return status > 0;
        }

        public bool UpdateRoomCategoryEnable(string id, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[room_category] SET Enable=0 WHERE RoomCategoryId = @id";
            int status = this._connection.Execute(query, new { id }, transaction);
            return status > 0;
        }
        #endregion

        #region ImageRoom
        public bool InsertImageRoom(ImageRoom model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[image_room] ([ImageRoomId],[RoomCategoryId],[LinkImage]) VALUES (@ImageRoomId,@RoomCategoryId,@LinkImage)";
            int status = this._connection.Execute(query, model, transaction);
            return status > 0;
        }

        public List<ImageRoom> GetListImageRoom(string id, IDbTransaction transaction = null)
        {
            string query = "select * from image_room where RoomCategoryId=@id";
            return this._connection.Query<ImageRoom>(query, new { id }, transaction).ToList();
        }

        public ImageRoom GetImageRoomById(string id, IDbTransaction transaction = null)
        {
            string query = "select * from image_room where ImageRoomId = @id";
            return this._connection.Query<ImageRoom>(query, new { id }, transaction).FirstOrDefault();
        }

        public bool DeleteImageRoom(string id, IDbTransaction transaction = null)
        {
            string query = "delete from image_room where ImageRoomId = @id";
            int status = this._connection.Execute(query, new { id }, transaction);
            return status > 0;
        }
        #endregion
    }
}