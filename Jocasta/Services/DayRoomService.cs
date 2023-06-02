using Dapper;
using Jocasta.Models;
using Jocasta.Providers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Jocasta.Services
{
    public class DayRoomService : BaseService
    {
        public DayRoomService() : base() { }
        public DayRoomService(IDbConnection db) : base(db) { }  
        public DayRoom GetDayRoomByRoomAndDate(string roomId, long date, IDbTransaction transaction = null)
        {
            string query = "select * from [day_room] where [RoomId]=@roomId and [DayTime]=@date";
            return this._connection.Query<DayRoom>(query, new { roomId, date }, transaction).FirstOrDefault();
        }

        public void UpdateDayRoom(DayRoom dayRoom, IDbTransaction transaction = null)
        {
            string query = "update [day_room] set [OrderDetailId]=@OrderDetailId, [Status]=@Status where [DayRoomId]=@DayRoomId";
            int count = this._connection.Execute(query, dayRoom, transaction);
            if (count <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void UpdateDayRoomByOrderDetail(string orderDetailId, string status, IDbTransaction transaction = null)
        {
            string query = "update [day_room] set [OrderDetailId]=NULL, [Status]=@status where [OrderDetailId]=@orderDetailId";
            int count = this._connection.Execute(query, new { orderDetailId , status }, transaction);
            if (count <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public List<Room> GetListRoomByOrder(string orderDetailId, IDbTransaction transaction = null)
        {
            string query = "select DISTINCT r.RoomId, r.Name from [day_room] dr left join [room] r on dr.RoomId = r.RoomId where dr.OrderDetailId = @orderDetailId";
            return this._connection.Query<Room>(query, new { orderDetailId }, transaction).ToList();
        }
    }
}