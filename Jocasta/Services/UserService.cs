﻿using Dapper;
using Jocasta.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Policy;
using System.Web;

namespace Jocasta.Services
{
    public class UserService : BaseService
    {
        public UserService() : base() { }
        public UserService(IDbConnection db) : base(db) { }

        #region user
        public List<User> GetListUser(IDbTransaction transaction = null)
        {
            string query = "select * from [user]";
            return this._connection.Query<User>(query, null, transaction).ToList();
        }

        public User GetUserById(string id, IDbTransaction transaction = null)
        {
            string query = "select * from [user] where UserId = @id";
            return this._connection.Query<User>(query, new { id }, transaction).FirstOrDefault();
        }

        public User GetUserByUserName(string userName, IDbTransaction transaction = null)
        {
            string query = "select * from [user] where Account = @userName or Phone = @userName";
            return this._connection.Query<User>(query, new { userName }, transaction).FirstOrDefault();
        }

        public bool InsertNewUser(User user, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[user] ([UserId],[Name],[Account],[Password],[Email],[Phone],[Enable],[CreateTime]) " +
                "VALUES (@UserId, @Name, @Account, @Password, @Email, @Phone, @Enable, @CreateTime)";
            int status = this._connection.Execute(query, user, transaction);
            return status > 0;
        }

        public string CheckDuplicateUser(string keyword, IDbTransaction transaction = null)
        {
            string query = "select * from [user] where Phone = @keyword or Email = @keyword or Account = @keyword";
            return this._connection.Query<string>(query, new { keyword }, transaction).FirstOrDefault();
        }

        public User GetUserByToken(string Token, IDbTransaction transaction = null)
        {
            string query = "select u.* from [user] u join [user_token] ut on u.UserId = ut.UserId where ut.Token = @Token";
            return this._connection.Query<User>(query, new { Token }, transaction).FirstOrDefault();
        }

        public void CheckAccountExist(string account, string userId, IDbTransaction transaction = null)
        {
            string query = "select count(*) from [user] where Account = @account and Account <> '' and UserId <> @userId";
            int count = this._connection.Query<int>(query, new { account = account, userId = userId }, transaction).FirstOrDefault();
            if (count > 0) throw new Exception("Tên đăng nhập đã tồn tại.");
        }
        public void CheckEmailExit(string email, string userId, IDbTransaction transaction = null)
        {
            string query = "select count(*) from [user] where Email = @email and Email <> '' and UserId <> @userId";
            int count = this._connection.Query<int>(query, new { email = email, userId = userId }, transaction).FirstOrDefault();
            if (count > 0) throw new Exception("Email đã tồn tại.");
        }
        public void CheckPhoneExit(string phone, string userId, IDbTransaction transaction = null)
        {
            string query = "select count(*) from [user] where Phone = @phone and Phone <> '' and UserId <> @userId";
            int count = this._connection.Query<int>(query, new { phone = phone, userId = userId }, transaction).FirstOrDefault();
            if (count > 0) throw new Exception("Số điện thoại đã tồn tại.");
        }

        public void UpdateUserInfo(User user, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[user] SET [Name]=@Name,[Account]=@Account,[Email]=@Email,[Phone]=@Phone,[Avatar]=@Avatar WHERE [UserId]=@UserId";
            int count = this._connection.Execute(query, user, transaction);
            if (count > 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
        #endregion

        #region User Token
        public bool InsertUserToken(UserToken userToken, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[user_token] ([UserTokenId],[UserId],[Token],[CreateTime],[ExpireTime]) VALUES (@UserTokenId,@UserId,@Token,@CreateTime,@ExpireTime)";
            int status = this._connection.Execute(query, userToken, transaction);
            return status > 0;
        }

        public UserToken GetUserTokenByUserId(string userId, IDbTransaction transaction = null)
        {
            string query = "select * from [user_token] where [UserId] = @userId";
            return this._connection.Query<UserToken>(query, new { userId }, transaction).FirstOrDefault();
        }

        public bool UpdateUserToken(UserToken userToken, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[user_token] SET [Token] = @Token, [CreateTime] = @CreateTime, [ExpireTime] = @ExpireTime WHERE [UserId] = @UserId";
            int status = this._connection.Execute(query, userToken, transaction);
            return status > 0;
        }

        public UserToken GetUserToken(string token, IDbTransaction transaction = null)
        {
            string query = "select top 1 * from [user_token] where Token=@token";
            return this._connection.Query<UserToken>(query, new { token }, transaction).FirstOrDefault();
        }

        public void RemoveUserToken(string token, IDbTransaction transaction = null)
        {
            string query = "update [user_token] set Token=NULL where Token=@token";
            int status = this._connection.Execute(query, new { token = token }, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        #endregion
    }
}