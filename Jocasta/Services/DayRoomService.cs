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
            int status = this._connection.Execute(query, dayRoom, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
            
    }
}