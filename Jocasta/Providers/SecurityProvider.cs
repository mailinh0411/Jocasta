using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;

namespace Jocasta.Providers
{
    public class SecurityProvider
    {
        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString().ToLower();
            }
        }
        public static string EncodePassword(string userId, string password)
        {
            try
            {
                userId = CreateMD5(userId);
                password = CreateMD5(userId[userId.Length - 1] + password + userId[0]);
                password = CreateMD5(userId[userId.Length - 2] + password + userId[1]);
                return password;
            }
            catch (Exception ex) { return null; }
        }

        public static string CreateToken(string account, string password, string deviceId)
        {
            try
            {
                string token = "";
                token = Guid.NewGuid().ToString();
                token = Base64Encode(token);

                //Mã hoá tài khoản
                token = token.Substring(0, 10) + "4c(0@n7" + account + "4c(0@n7" + token.Substring(10, token.Length - 10);
                token = Base64Encode(token);

                //Mã hoá password
                token = token.Substring(0, 10) + "9a$$W07d" + password + "9a$$W07d" + token.Substring(10, token.Length - 10);
                token = Base64Encode(token);

                //Mã hoá DeviceId
                token = token.Substring(0, 10) + "d3V1@31D" + deviceId + "d3V1@31D" + token.Substring(10, token.Length - 10);
                token = Base64Encode(token);

                //Ma hoa lan cuoi
                for (int i = 0; i < 3; i++)
                {
                    token = token + Guid.NewGuid().ToString();
                    token = Base64Encode(token);
                }

                token = "Utao " + token;
                return token;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /*public static User DecodeToken(string token)
        {
            try
            {
                User user = new User();
                String[] tokenSplit;

                for (int i = 0; i < 3; i++)
                {
                    token = Base64Decode(token);
                    token = token.Substring(0, token.Length - 36);
                }

                token = Base64Decode(token);
                tokenSplit = token.Split(new[] { "d3V1@31D" }, StringSplitOptions.None);

                //customer.DeviceId = tokenSplit[1];
                token = tokenSplit[0] + tokenSplit[2];

                token = Base64Decode(token);
                tokenSplit = token.Split(new[] { "9a$$W07d" }, StringSplitOptions.None);
                //customer.Password = tokenSplit[1];
                token = tokenSplit[0] + tokenSplit[2];

                token = Base64Decode(token);
                tokenSplit = token.Split(new[] { "4c(0@n7" }, StringSplitOptions.None);
                user.UserId = tokenSplit[1];

                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }*/

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        /*public static UserAdmin GetUserAdminByToken(HttpRequestMessage requestMessage, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            if (requestMessage == null) return null;
            if (requestMessage.Headers == null) return null;
            if (requestMessage.Headers.Authorization == null) return null;
            string token = requestMessage.Headers.Authorization.ToString();
            UserAdminService userService = new UserAdminService(connection);
            return userService.GetUserAdminByToken(token, transaction);
        }

        public static User GetUserByToken(HttpRequestMessage requestMessage, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            if (requestMessage == null) return null;
            if (requestMessage.Headers == null) return null;
            if (requestMessage.Headers.Authorization == null) return null;
            string token = requestMessage.Headers.Authorization.ToString();
            UserService userService = new UserService(connection);
            return userService.GetUserByToken(token, transaction);
        }*/
    }
}