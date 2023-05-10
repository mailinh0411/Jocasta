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
    public class AdminSystemWalletService : BaseService
    {
        public AdminSystemWalletService() { }
        public AdminSystemWalletService(IDbConnection db) : base(db) { }
        public void UpdateRevenueSystem(decimal revenue, IDbTransaction transaction = null)
        {
            string query = "update [dbo].[system_wallet] set Balance = @revenue where SystemWalletId = 'revenue'";
            int count = this._connection.Execute(query, new { revenue }, transaction);
            if (count <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
    }
}