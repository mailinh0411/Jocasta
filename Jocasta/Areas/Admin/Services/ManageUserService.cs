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
    public class ManageUserService : BaseService
    {
        public ManageUserService() : base() { }
        public ManageUserService(IDbConnection db) : base(db) { }
        public ListUserView GetListUser(string keyword, string enable, int page, int pageSize, IDbTransaction transaction = null)
        {
            string querySelect = "select UserId, Name,Avatar, Account, Email, Phone,Enable, CreateTime";
            string queryCount = "select count(*)";
            string query = " from [user] where 1=1";
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = "%" + keyword.Replace(" ", "%") + "%";
                query += " and (Account like @keyword or Phone like @keyword or Email like @keyword)";
            }
            if (!string.IsNullOrEmpty(enable))
            {
                query += " and Enable = @enable";
            }

            ListUserView list = new ListUserView();
            int totalRow = this._connection.Query<int>(queryCount + query, new { keyword, enable }, transaction).FirstOrDefault();

            if (totalRow > 0)
            {
                list.TotalPage = (int)Math.Ceiling((decimal)totalRow / (decimal)pageSize);
            }
            int skip = (page - 1) * pageSize;

            query += " order by CreateTime desc OFFSET " + skip + " ROWS FETCH NEXT " + pageSize + " ROWS ONLY";
            list.List = new List<User>();
            list.List = this._connection.Query<User>(querySelect + query, new { keyword, enable }, transaction).ToList();
            return list;
        }

        public User GetUserById(string id, IDbTransaction transaction = null)
        {
            string query = "select UserId, Name,Avatar, Account, Email, Phone,Enable, CreateTime from [user] where UserId = @id";
            return this._connection.Query<User>(query, new { id }, transaction).FirstOrDefault();
        }
        public bool CheckAccountExist(string account, string userId, IDbTransaction transaction = null)
        {
            string query = "select count(*) from [user] where Account = @account and Account <> '' and UserId <> @userId";
            int count = this._connection.Query<int>(query, new { account = account, userId = userId }, transaction).FirstOrDefault();
            return count > 0;
        }
        public bool CheckEmailExit(string email, string userId, IDbTransaction transaction = null)
        {
            string query = "select count(*) from [user] where Email = @email and Email <> '' and UserId <> @userId";
            int count = this._connection.Query<int>(query, new { email = email, userId = userId }, transaction).FirstOrDefault();
            return count > 0;
        }
        public bool UpdateUser(User model, IDbTransaction transaction = null)
        {
            string query = "UPDATE [user] SET Name = @Name,Avatar = @Avatar, Account = @Account, Email = @Email, Phone = @Phone WHERE UserId=@UserId";
            int status = this._connection.Execute(query, model, transaction);
            return status > 0;
        }

        public bool ChangeUserEnable(User model, IDbTransaction transaction = null)
        {
            string query = "UPDATE [user] SET Enable = @Enable WHERE UserId = @UserId";
            int status = this._connection.Execute(query, model, transaction);
            return status > 0;
        }

        public bool DeleteUser(string id, IDbTransaction transaction = null)
        {
            string query = "DELETE FROM [user] WHERE UserId = @id";
            int status = this._connection.Execute(query, new { id }, transaction);
            return status > 0;
        }

        public int GetCountUser(IDbTransaction transaction = null)
        {
            string query = "select count(*) from [user]";
            return this._connection.Query<int>(query, transaction).FirstOrDefault();
        }
    }
}