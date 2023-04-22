using Jocasta.Models;
using Jocasta.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace Jocasta.Providers
{
    public class UserProvider
    {
        public static User GetUserFromRequestHeader(HttpRequestMessage request, IDbConnection connect = null, IDbTransaction transaction = null)
        {
            if (request == null) return null;
            string token = request.Headers.GetValues("Authorization").FirstOrDefault();
            if (string.IsNullOrEmpty(token)) return null;

            UserService userService = new UserService(connect);

            UserToken userToken = userService.GetUserToken(token, transaction);
            if (userToken == null) return null;
            long now = HelperProvider.GetSeconds();
            if (userToken.ExpireTime <= now)
            {
                userService.RemoveUserToken(userToken.Token, transaction);
                return null;
            }

            User user = userService.GetUserByToken(userToken.Token, transaction);

            return user;
        }
    }
}