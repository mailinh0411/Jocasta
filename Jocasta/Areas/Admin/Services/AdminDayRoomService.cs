using Dapper;
using Jocasta.Models;
using Jocasta.Providers;
using Jocasta.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Jocasta.Areas.Admin.Services
{
    public class AdminDayRoomService : BaseService
    {
        public AdminDayRoomService() : base() { }
        public AdminDayRoomService(IDbConnection db) : base(db) { }
        public ListDayRoomView GetListDayRoom(string keyword, string status, string startDate, string endDate, int page, int pageSize, IDbTransaction transaction = null)
        {
            string querySelect = "select dr.DayRoomId, dr.RoomId, dr.OrderDetailId, dr.Status, dr.DayTime, r.Name as RoomName";
            string queryCount = "select count(*)";
            string query = " from [day_room] dr left join [room] r on dr.RoomId = r.RoomId where 1=1";
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = "%" + keyword.Replace(" ", "%") + "%";
                query += " and (r.Name like @keyword)";
            }

            if (startDate != null && startDate != "")
            {
                DateTime StartDate = Convert.ToDateTime(startDate);
                long start = HelperProvider.GetSeconds(new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, 0, 0, 0));
                query += " and dr.DayTime >= " + start;
            }

            if (endDate != null && endDate != "")
            {
                DateTime EndDate = Convert.ToDateTime(endDate);
                long end = HelperProvider.GetSeconds(new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, 23, 59, 59));
                query += " and dr.DayTime <= " + end;
            }
            if (!string.IsNullOrEmpty(status))
            {
                query += " and dr.Status = @status";
            }

            ListDayRoomView list = new ListDayRoomView();
            int totalRow = this._connection.Query<int>(queryCount + query, new { keyword, status, startDate, endDate }, transaction).FirstOrDefault();

            if (totalRow > 0)
            {
                list.TotalPage = (int)Math.Ceiling((decimal)totalRow / (decimal)pageSize);
            }
            int skip = (page - 1) * pageSize;

            query += " order by dr.DayTime desc OFFSET " + skip + " ROWS FETCH NEXT " + pageSize + " ROWS ONLY";
            list.List = new List<DayRoomModel>();
            list.List = this._connection.Query<DayRoomModel>(querySelect + query, new { keyword, status, startDate, endDate }, transaction).ToList();
            return list;
        }

        public object GetListDayRoomByDay(string keyword, string status, long dayTime, int floor, IDbTransaction transaction = null)
        {
            string querySelect = "select dr.DayRoomId, dr.RoomId, dr.OrderDetailId, dr.Status, dr.DayTime, r.Name as RoomName, r.Floor, o.CheckIn, o.CheckOut, o.Name";            
            string query = " from [day_room] dr left join [room] r on dr.RoomId = r.RoomId left join [order_detail] od on dr.OrderDetailId = od.OrderDetailId left join [order] o on od.OrderId = o.OrderId where r.Floor = @floor and r.Enable=1";
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = "%" + keyword.Replace(" ", "%") + "%";
                query += " and (r.Name like @keyword)";
            }

            if (dayTime != null && dayTime != 0)
            {
                //DateTime StartDate = Convert.ToDateTime(startDate);
                DateTime StartDate = HelperProvider.GetDateTime(dayTime);
                long start = HelperProvider.GetSeconds(new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, 0, 0, 0));
                query += " and dr.DayTime = " + start;
            }

            if (!string.IsNullOrEmpty(status))
            {
                query += " and dr.Status = @status";
            }

            query += " order by r.Name asc";
            return this._connection.Query<object>(querySelect + query, new {keyword, status, dayTime, floor}, transaction).ToList();
        }

        public DayRoom GetDayRoomById(string id, IDbTransaction transaction = null)
        {
            string query = "select * from [day_room] where DayRoomId = @id";
            return this._connection.Query<DayRoom>(query, new { id }, transaction).FirstOrDefault();
        }

        public bool InsertDayRoom(DayRoom model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[day_room] ([DayRoomId],[RoomId],[OrderDetailId],[DayTime],[Status]) VALUES (@DayRoomId,@RoomId,@OrderDetailId,@DayTime,@Status)";
            int status = this._connection.Execute(query, model, transaction);
            return status > 0;
        }

        public bool UpdateDayRoom(DayRoom model, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[day_room] SET [RoomId]=@RoomId,[DayTime]=@DayTime WHERE [DayRoomId]=@DayRoomId";
            int status = this._connection.Execute(query, model, transaction);
            return status > 0;
        }

        public bool DeleteDayRoom(string id, IDbTransaction transaction = null)
        {
            string query = "DELETE FROM [dbo].[day_room] WHERE [DayRoomId]=@id";
            int status = this._connection.Execute(query, new { id }, transaction);
            return status > 0;
        }

        public bool ChangeStatusDayRoom(string id, string status, IDbTransaction transaction = null)
        {
            string query = "update [dbo].[day_room] set Status = @status where [DayRoomId]=@id";
            int dem = this._connection.Execute(query, new { id, status }, transaction);
            return dem > 0;
        }

        public List<DayRoomModel> GetListDayRoomByDetail(string orderDetailId, IDbTransaction transaction = null)
        {
            string query = "select dr.*, r.Name as RoomName from [day_room] dr left join [room] r on dr.RoomId = r.RoomId where dr.OrderDetailId = @orderDetailId";
            return this._connection.Query<DayRoomModel>(query, new { orderDetailId }, transaction).ToList();
        }

        public List<Room> GetListRoomByOrder(string orderDetailId, IDbTransaction transaction = null)
        {
            string query = "select DISTINCT r.RoomId, r.Name from [day_room] dr left join [room] r on dr.RoomId = r.RoomId where dr.OrderDetailId = @orderDetailId";
            return this._connection.Query<Room>(query, new { orderDetailId }, transaction).ToList();
        }

        public void UpdateDayRoomByOrderDetail(string orderDetailId, string status, IDbTransaction transaction = null)
        {
            string query = "update [day_room] set [OrderDetailId]=NULL, [Status]=@status where [OrderDetailId]=@orderDetailId";
            int count = this._connection.Execute(query, new { orderDetailId, status }, transaction);
            if (count <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public DayRoom GetDayRoomByDayAndRoom(string roomId, long day, IDbTransaction transaction = null)
        {
            string query = "select * from day_room where RoomId=@roomId and DayTime = @day";
            return this._connection.Query<DayRoom>(query, new {roomId, day}, transaction).FirstOrDefault();
        }
    }
}