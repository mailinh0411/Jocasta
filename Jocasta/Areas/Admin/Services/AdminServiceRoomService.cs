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
    public class AdminServiceRoomService : BaseService
    {
        public AdminServiceRoomService() : base() { }
        public AdminServiceRoomService(IDbConnection db) : base(db) { }

        public ListServiceView GetListService(string keyword, int page, int pageSize, IDbTransaction transaction = null)
        {
            string querySelect = "select *";
            string queryCount = "select count(*)";
            string query = " from [service] where 1=1";
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = "%" + keyword.Replace(" ", "%") + "%";
                query += " and Name like @keyword";
            }

            ListServiceView list = new ListServiceView();
            int totalRow = this._connection.Query<int>(queryCount + query, new { keyword }, transaction).FirstOrDefault();

            if (totalRow > 0)
            {
                list.TotalPage = (int)Math.Ceiling((decimal)totalRow / (decimal)pageSize);
            }
            int skip = (page - 1) * pageSize;

            query += " order by CreateTime desc OFFSET " + skip + " ROWS FETCH NEXT " + pageSize + " ROWS ONLY";
            list.List = new List<Service>();
            list.List = this._connection.Query<Service>(querySelect + query, new { keyword }, transaction).ToList();
            return list;
        }

        public Service GetServiceById(string id, IDbTransaction transaction = null)
        {
            string query = "select * from [service] where ServiceId = @id";
            return this._connection.Query<Service>(query, new { id }, transaction).FirstOrDefault();
        }

        public bool InsertService(Service model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[service] ([ServiceId],[Name],[Price],[Image],[Enable],[Description],[CreateTime]) VALUES (@ServiceId,@Name,@Price,@Image,@Enable,@Description,@CreateTime)";
            int status = this._connection.Execute(query, model, transaction);
            return status > 0;
        }

        public bool UpdateService(Service model, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[service] SET [Name]=@Name,[Price]=@Price,[Image]=@Image,[Description]=@Description WHERE [ServiceId]=@ServiceId";
            int status = this._connection.Execute(query, model, transaction);
            return status > 0;
        }

        public bool DeleteService(string id, IDbTransaction transaction = null)
        {
            string query = "delete from [service] where [ServiceId]=@id";
            int status = this._connection.Execute(query, new { id }, transaction);
            return status > 0;
        }
    }
}