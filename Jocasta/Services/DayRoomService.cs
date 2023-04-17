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
        public DayRoom CheckDayRoomAvailable(string roomId, long date, IDbTransaction transaction = null)
        {
            string query = "select * from [day_room] where [RoomId]=@roomId and [DayTime]=@date";
            return this._connection.Query<DayRoom>(query, new { roomId, date }, transaction).FirstOrDefault();
        }
            
    }
}