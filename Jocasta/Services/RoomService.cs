using Dapper;
using Jocasta.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Jocasta.Services
{
    public class RoomService : BaseService
    {
        public RoomService() : base() { }
        public RoomService(IDbConnection db) : base(db) { }

        public List<Room> GetListRoomByCategory(string category, IDbTransaction transaction = null)
        {
            string query = "select * from [room] where [RoomCategoryId] = @category";
            return this._connection.Query<Room>(query, new { category }, transaction).ToList();
        }
    }
}