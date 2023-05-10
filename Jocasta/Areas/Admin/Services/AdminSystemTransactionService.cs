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
    public class AdminSystemTransactionService : BaseService
    {
        public AdminSystemTransactionService() : base() { }
        public AdminSystemTransactionService(IDbConnection db) : base(db) { }

        #region SystemTransaction
        public void InsertSystemTransaction(SystemTransaction model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[system_transaction] ([SystemTransactionId],[Amount],[Note],[CreateTime]) VALUES (@SystemTransactionId,@Amount,@Note,@CreateTime)";
            int count = this._connection.Execute(query, model, transaction);
            if (count <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
        #endregion

        
    }
}