using Dapper;
using Jocasta.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Jocasta.Services
{
    public class ServiceRoomService : BaseService
    {
        public ServiceRoomService() : base() { }
        public ServiceRoomService(IDbConnection db) : base(db) { }

        public List<Service> GetListServiceForHomePage(IDbTransaction transaction = null)
        {
            string query = "select Top(6) * from [service] where Enable=1 ORDER BY CreateTime DESC";
            return this._connection.Query<Service>(query, null, transaction).ToList();
        }

        public List<Service> GetListService(IDbTransaction transaction = null)
        {
            string query = "select * from [service] where Enable=1";
            return this._connection.Query<Service>(query, transaction).ToList();
        }
    }
}