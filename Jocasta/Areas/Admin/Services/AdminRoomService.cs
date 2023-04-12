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
    public class AdminRoomService : BaseService
    {
        public AdminRoomService() : base() { }
        public AdminRoomService(IDbConnection db) : base(db) { }

        public ListRoomView GetListRoom(string keyword, string roomCategory, string enable, int page, int pageSize, IDbTransaction transaction = null)
        {
            string querySelect = "select r.RoomId, r.Name, r.Floor, r.RoomCategoryId, r.Enable, r.CreateTime, rc.Name as RoomCategoryName";
            string queryCount = "select count(*)";
            string query = " from [room] r left join [room_category] rc on r.RoomCategoryId = rc.RoomCategoryId where 1=1";
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = "%" + keyword.Replace(" ", "%") + "%";
                query += " and (r.Name like @keyword or r.Floor like @keyword)";
            }
            if (!string.IsNullOrEmpty(roomCategory))
            {
                query += " and r.RoomCategoryId = @roomCategory";
            }
            if (!string.IsNullOrEmpty(enable))
            {
                query += " and r.Enable = @enable";
            }

            ListRoomView list = new ListRoomView();
            int totalRow = this._connection.Query<int>(queryCount + query, new { keyword, roomCategory, enable }, transaction).FirstOrDefault();

            if (totalRow > 0)
            {
                list.TotalPage = (int)Math.Ceiling((decimal)totalRow / (decimal)pageSize);
            }
            int skip = (page - 1) * pageSize;

            query += " order by r.CreateTime desc OFFSET " + skip + " ROWS FETCH NEXT " + pageSize + " ROWS ONLY";
            list.List = new List<Room>();
            list.List = this._connection.Query<Room>(querySelect + query, new { keyword, roomCategory, enable }, transaction).ToList();
            return list;
        }

        public List<Room> GetListRoombyCategory(string id, IDbTransaction transaction = null)
        {
            string query = "select * from [room] where RoomCategoryId = @id";
            List<Room> list = this._connection.Query<Room>(query, new { id }, transaction).ToList();
            return list;
        }

        public List<Room> GetRooms(IDbTransaction transaction = null)
        {
            string query = "select * from [room]";
            return this._connection.Query<Room>(query, transaction).ToList();
        }

        public Room GetRoomById(string id, IDbTransaction transaction = null)
        {
            string query = "select * from [dbo].[room] where RoomId = @id";
            return this._connection.Query<Room>(query, new { id }, transaction).FirstOrDefault();
        }

        public bool InsertRoom(Room room, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[room] ([RoomId],[Name],[Floor],[RoomCategoryId],[Enable],[CreateTime]) VALUES (@RoomId,@Name,@Floor,@RoomCategoryId,@Enable,@CreateTime)";
            int status = this._connection.Execute(query, room, transaction);
            return status > 0;
        }

        public bool UpdateRoom(Room room, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[room] SET [Name]=@Name,[Floor]=@Floor,[RoomCategoryId]=@RoomCategoryId WHERE [RoomId]=@RoomId";
            int status = this._connection.Execute(query, room, transaction);
            return status > 0;
        }

        public bool DeleteRoom(string id, IDbTransaction transaction = null)
        {
            string query = "delete from [dbo].[room] where [RoomId]=@id";
            int status = this._connection.Execute(query, new { id }, transaction);
            return status > 0;
        }

        public bool ChangeEnableRoom(string id, bool enable, IDbTransaction transaction = null)
        {
            string query = "update [dbo].[room] set Enable = @enable where [RoomId]=@id";
            int status = this._connection.Execute(query, new { id, enable }, transaction);
            return status > 0;
        }
    }
}