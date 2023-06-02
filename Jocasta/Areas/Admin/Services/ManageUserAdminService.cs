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
    public class ManageUserAdminService : BaseService
    {
        public ManageUserAdminService() : base() { }
        public ManageUserAdminService(IDbConnection db) : base(db) { }
        public UserAdmin GetUserAdminByAccount(string account, IDbTransaction transaction = null)
        {
            string query = "select * from user_admin where Account=@account";
            return this._connection.Query<UserAdmin>(query, new { account = account }, transaction).FirstOrDefault();
        }

        public UserAdmin GetUserAdminByToken(string token, IDbTransaction transaction = null)
        {
            string query = "select * from user_admin where Token=@token";
            return this._connection.Query<UserAdmin>(query, new { token = token }, transaction).FirstOrDefault();
        }

        public void UpdateUserAdminToken(string userAdminId, string token, IDbTransaction transaction = null)
        {
            string query = "update user_admin set Token=@token where UserAdminId=@userAdminId";
            int status = this._connection.Execute(query, new { userAdminId, token }, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public bool UpdateUserAdminPassword(string userAdminId, string password, IDbTransaction transaction = null)
        {
            string query = "update user_admin set Password=@password where UserAdminId=@userAdminId";
            int status = this._connection.Execute(query, new { userAdminId, password }, transaction);
            return status > 0;
        }
        public List<object> GetListUserAdmin(IDbTransaction transaction = null)
        {
            string query = "select UserAdminId, Account, Name from user_admin";
            return this._connection.Query<object>(query, null, transaction).ToList();
        }
        public bool InsertUserAdmin(UserAdmin model, IDbTransaction transaction = null)
        {
            string insert = "INSERT INTO [dbo].[user_admin] ([UserAdminId],[Avatar],[Account],[Name],[Email],[Phone],[Enable],[Password],[Token],[CreateTime]) VALUES (@UserAdminId,@Avatar,@Account,@Name,@Email,@Phone,@Enable,@Password,@Token,@CreateTime)";
            int status = this._connection.Execute(insert, model, transaction);
            return status > 0;
        }
        public bool UpdateUserAdmin(UserAdmin model, IDbTransaction transaction = null)
        {
            string update = "UPDATE [dbo].[user_admin] SET [Account] = @Account,[Avatar]=@Avatar,[Name] = @Name,[Email]=@Email,[Phone]=@Phone,[Password] = @Password,[Token] = @Token,[CreateTime] = @CreateTime WHERE [UserAdminId] = @UserAdminId";
            int status = this._connection.Execute(update, model, transaction);
            return status > 0;
        }
        public UserAdmin GetUserAdminById(string adminId, IDbTransaction transaction = null)
        {
            string query = "select * from user_admin where UserAdminId=@adminId";
            return this._connection.Query<UserAdmin>(query, new { adminId = adminId }, transaction).FirstOrDefault();
        }
        public bool DeleteUserAdmin(string adminId, IDbTransaction transaction = null)
        {
            string delete = "DELETE FROM [dbo].[user_admin] WHERE [UserAdminId] = @adminId";
            int status = this._connection.Execute(delete, new { adminId = adminId }, transaction);
            return status > 0;
        }

        public ListUserAdminView GetListUserAdmin(string keyword, string enable, int page, int pageSize, IDbTransaction transaction = null)
        {
            string querySelect = "select UserAdminId, Name,Avatar, Account, Email, Phone,Enable, CreateTime";
            string queryCount = "select count(*)";
            string query = " from [user_admin] where 1=1";
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = "%" + keyword.Replace(" ", "%") + "%";
                query += " and (Account like @keyword or Phone like @keyword or Email like @keyword)";
            }
            if (!string.IsNullOrEmpty(enable))
            {
                query += " and Enable = @enable";
            }

            ListUserAdminView list = new ListUserAdminView();
            int totalRow = this._connection.Query<int>(queryCount + query, new { keyword, enable }, transaction).FirstOrDefault();

            if (totalRow > 0)
            {
                list.TotalPage = (int)Math.Ceiling((decimal)totalRow / (decimal)pageSize);
            }
            int skip = (page - 1) * pageSize;

            query += " order by CreateTime desc OFFSET " + skip + " ROWS FETCH NEXT " + pageSize + " ROWS ONLY";
            list.List = new List<UserAdmin>();
            list.List = this._connection.Query<UserAdmin>(querySelect + query, new { keyword, enable }, transaction).ToList();
            return list;
        }

        public bool CheckAccountExist(string account, string userAdminId, IDbTransaction transaction = null)
        {
            string query = "select count(*) from [user_admin] where Account = @account and Account <> '' and UserAdminId <> @userAdminId";
            int count = this._connection.Query<int>(query, new { account = account, userAdminId = userAdminId }, transaction).FirstOrDefault();
            return count > 0;
        }
        public bool CheckEmailExit(string email, string userAdminId, IDbTransaction transaction = null)
        {
            string query = "select count(*) from [user_admin] where Email = @email and Email <> '' and UserAdminId <> @userAdminId";
            int count = this._connection.Query<int>(query, new { email = email, userAdminId = userAdminId }, transaction).FirstOrDefault();
            return count > 0;
        }

        public bool CheckPhoneExit(string phone, string userAdminId, IDbTransaction transaction = null)
        {
            string query = "select count(*) from [user_admin] where Phone = @phone and Phone <> '' and UserAdminId <> @userAdminId";
            int count = this._connection.Query<int>(query, new { phone = phone, userAdminId = userAdminId }, transaction).FirstOrDefault();
            return count > 0;
        }

        public string CheckDuplicateUser(string keyword, IDbTransaction transaction = null)
        {
            string query = "select * from [user] where Phone = @keyword or Email = @keyword or Account = @keyword";
            return this._connection.Query<string>(query, new { keyword }, transaction).FirstOrDefault();
        }

        public bool ChangeUserAdminEnable(UserAdmin model, IDbTransaction transaction = null)
        {
            string query = "UPDATE [user_admin] SET Enable = @Enable WHERE UserAdminId = @UserAdminId";
            int status = this._connection.Execute(query, model, transaction);
            return status > 0;
        }

        public void RemoveUserAdminToken(string token, IDbTransaction transaction = null)
        {
            string query = "update [user_admin] set Token=NULL where Token=@token";
            int status = this._connection.Execute(query, new { token = token }, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        
    }
}