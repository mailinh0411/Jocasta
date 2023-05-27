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
            string insert = "INSERT INTO [dbo].[user_admin] ([UserAdminId],[Account],[Name],[Password],[Token],[CreateTime]) VALUES (@UserAdminId,@Account,@Name,@Password,@Token,@CreateTime)";
            int status = this._connection.Execute(insert, model, transaction);
            return status > 0;
        }
        public bool UpdateUserAdmin(UserAdmin model, IDbTransaction transaction = null)
        {
            string update = "UPDATE [dbo].[user_admin] SET [Account] = @Account,[Name] = @Name,[Password] = @Password,[Token] = @Token,[CreateTime] = @CreateTime WHERE [UserAdminId] = @UserAdminId";
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
    }
}