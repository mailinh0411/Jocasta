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
    public class AdminNewService : BaseService
    {
        public AdminNewService() : base() { }
        public AdminNewService(IDbConnection db) : base(db) { }
        public ListNewView GetListNew(string keyword, int page, int pageSize, IDbTransaction transaction = null)
        {
            string querySelect = "select *";
            string queryCount = "select count(*)";
            string query = " from [new] where 1=1";
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = "%" + keyword.Replace(" ", "%") + "%";
                query += " and (Title like @keyword)";
            }

            ListNewView list = new ListNewView();
            int totalRow = this._connection.Query<int>(queryCount + query, new { keyword }, transaction).FirstOrDefault();

            if (totalRow > 0)
            {
                list.TotalPage = (int)Math.Ceiling((decimal)totalRow / (decimal)pageSize);
            }
            int skip = (page - 1) * pageSize;

            query += " order by CreateTime desc OFFSET " + skip + " ROWS FETCH NEXT " + pageSize + " ROWS ONLY";
            list.List = new List<New>();
            list.List = this._connection.Query<New>(querySelect + query, new { keyword }, transaction).ToList();
            return list;
        }

        public bool InsertNew(New model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[new] ([NewId],[Image],[Content],[Title],[UserAdminId],[CreateTime]) " +
                "VALUES (@NewId,@Image,@Content,@Title,@UserAdminId,@CreateTime)";
            int status = this._connection.Execute(query, model, transaction);
            return status > 0;
        }

        public bool UpdateNew(New model, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[new] SET [Image]=@Image,[Content]=@Content,[Title]=@Title,[UserAdminId]=@UserAdminId WHERE [NewId]=@NewId";
            int status = this._connection.Execute(query, model, transaction);
            return status > 0;
        }

        public bool DeleteNew(string id, IDbTransaction transaction = null)
        {
            string query = "DELETE FROM [dbo].[new] WHERE [NewId]=@id";
            int status = this._connection.Execute(query, new { id }, transaction);
            return status > 0;
        }
    }
}