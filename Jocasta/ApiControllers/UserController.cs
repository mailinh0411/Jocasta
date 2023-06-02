using Jocasta.Filters;
using Jocasta.Models;
using Jocasta.Providers;
using Jocasta.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Jocasta.ApiControllers
{
    public class UserController : ApiBaseController
    {
        // GET: User
        [HttpGet]
        public JsonResult GetListUser()
        {
            UserService userService = new UserService();
            List<User> list = userService.GetListUser();
            return Success(list);
        }

        [HttpGet]
        [ApiTokenRequire]
        public JsonResult GetInforUser()
        {
            try
            {
                string token = Request.Headers.Authorization.ToString();
                UserService userService = new UserService();
                User user = userService.GetUserByToken(token);
                if (user == null) return Unauthorized();
                user.Password = "";
                return Success(user);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }


        [HttpPost]
        [AllowAnonymous]
        public JsonResult Login(User model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        UserService userService = new UserService(connect);

                        if (string.IsNullOrEmpty(model.Account) || string.IsNullOrEmpty(model.Password)) throw new Exception("Email/Tên đăng nhập/Mật khẩu không được để trống.");

                        User userLogin = userService.GetUserByUserName(model.Account, transaction);
                        if (userLogin == null) throw new Exception("Người dùng này không tồn tại");

                        if (userLogin.Enable == false) return Error("Tài khoản này đã bị khóa.");

                        string password = SecurityProvider.EncodePassword(userLogin.UserId, model.Password);
                        if (!userLogin.Password.Equals(password)) throw new Exception("Email/Tên đăng nhập/Mật khẩu không được để trống.");

                        string deviceId = Guid.NewGuid().ToString().ToLower();
                        string token = SecurityProvider.CreateToken(userLogin.UserId, userLogin.Password, deviceId);

                        DateTime now = DateTime.Now;
                        UserToken userToken = userService.GetUserTokenByUserId(userLogin.UserId, transaction);
                        userToken.Token = token;
                        userToken.CreateTime = HelperProvider.GetSeconds(now);
                        userToken.ExpireTime = HelperProvider.GetSeconds(now.AddDays(2));

                        if (!userService.UpdateUserToken(userToken, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                        
                        transaction.Commit();
                        return Success(new { token, deviceId });
                    }
                }
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }

        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult Register(User model)
        {
            try
            {
                using(var connect = BaseService.Connect())
                {
                    connect.Open();
                    using(var transaction = connect.BeginTransaction())
                    {
                        UserService userService = new UserService(connect);

                        if (string.IsNullOrEmpty(model.Name)) throw new Exception("Họ và tên người dùng không được để trống.");
                        if (string.IsNullOrEmpty(model.Account) || userService.CheckDuplicateUser(model.Account, transaction) != null) throw new Exception("Tên đăng nhập không được để trống.");
                        if (string.IsNullOrEmpty(model.Email) || userService.CheckDuplicateUser(model.Email, transaction) != null) throw new Exception("Email không được để trống.");
                        if (string.IsNullOrEmpty(model.Password)) throw new Exception("Password không được để trống.");
                        if (string.IsNullOrEmpty(model.Phone)) throw new Exception("Số điện thoại không được để trống.");

                        User user = new User();
                        user.UserId = Guid.NewGuid().ToString();
                        user.Name = model.Name;
                        user.Account = model.Account;
                        user.Email = model.Email;
                        user.Phone = model.Phone;
                        user.Password = SecurityProvider.EncodePassword(user.UserId, model.Password);
                        user.Enable = true;
                        DateTime now = DateTime.Now;
                        user.CreateTime = HelperProvider.GetSeconds(now);

                        if (!userService.InsertNewUser(user, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        UserToken userToken = new UserToken();
                        userToken.UserTokenId = Guid.NewGuid().ToString();
                        userToken.UserId = user.UserId;
                        userToken.CreateTime = HelperProvider.GetSeconds(now);
                        userToken.ExpireTime = HelperProvider.GetSeconds(now.AddDays(2));
                        if (!userService.InsertUserToken(userToken, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        transaction.Commit();
                        return Success();
                    }
                }
            }catch(Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult Logout()
        {
            try
            {
                string token = Request.Headers.Authorization.ToString();
                UserService userService = new UserService();
                userService.RemoveUserToken(token);
                return Success();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult UpdateUserInfo(User model)
        {
            try
            {
                using(var connect = BaseService.Connect())
                {
                    connect.Open();
                    using(var transaction = connect.BeginTransaction())
                    {
                        string token = Request.Headers.Authorization.ToString();
                        UserService userService = new UserService();
                        User user = userService.GetUserByToken(token);
                        if (user == null) return Unauthorized();

                        if (string.IsNullOrEmpty(user.Name)) throw new Exception("Tên người dùng không được để trống.");
                        user.Name = model.Name.Trim();
                        if (string.IsNullOrEmpty(user.Account)) throw new Exception("Tên đăng nhập không được để trống.");
                        userService.CheckAccountExist(model.Account, user.UserId, transaction);
                        user.Account = model.Account.Trim();

                        if(!string.IsNullOrEmpty(model.Email))
                        {
                            userService.CheckEmailExit(model.Email, user.UserId, transaction);
                            user.Email = model.Email.Trim();
                        }

                        if (!string.IsNullOrEmpty(model.Phone))
                        {
                            userService.CheckPhoneExit(model.Phone, user.UserId, transaction);
                            user.Phone = model.Phone.Trim();
                        }

                        if (!string.IsNullOrEmpty(model.Avatar))
                        {
                            string filename = Guid.NewGuid().ToString() + ".jpg";
                            var path = System.Web.HttpContext.Current.Server.MapPath("~" + Constant.USER_IMAGE_PATH + filename);
                            HelperProvider.Base64ToImage(model.Avatar, path);
                            if (!HelperProvider.DeleteFile(user.Avatar)) return Error();
                            user.Avatar = Constant.USER_IMAGE_URL + filename;
                        }

                        userService.UpdateUserInfo(user, transaction);

                        transaction.Commit();
                        return Success();
                    }
                }
            }catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult Insert()
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        UserService userService = new UserService(connect);

                        for (int i = 1; i < 17; i++)
                        {
                            
                            User user = new User();
                            user.UserId = Guid.NewGuid().ToString();
                            string index = i < 10 ? ('0' + i.ToString()) : i.ToString();                          
                            
                            string pass = "123";
                            user.Password = SecurityProvider.EncodePassword(user.UserId, pass);
                            user.Enable = true;
                            DateTime now = DateTime.Now;
                            user.CreateTime = HelperProvider.GetSeconds(now.AddDays(-i));

                            string[] ho = { "Nguyễn", "Hoàng", "Trần", "Bùi", "Đinh", "Trịnh", "Hồ", "Trương", "Nông", "Vũ", "Võ", "Mai", "Phạm", "Đỗ", "Lê" };
                            string[] dem = { "Văn", "Hưng", "Trọng", "Minh", "Ngọc", "Tiến", "Đức", "Việt", "Anh", "Quang", "Thành", "Trí", "Quốc", "Ánh", "Ngọc", "Thị", "Thanh", "Mỹ", "Khánh", "Linh", "Diễm", "Kim", "Tú", "Vân" };
                            string[] ten = { "Anh", "An", "Bảo", "Bình", "Bằng", "Bích", "Châu", "Chi", "Cường", "Cương", "Công", "Cảnh", "Chính", "Chung", "Diệp", "Duyên", "Diễm", "Dung", "Dũng", "Danh", "Dương", "Chiến", "Chương", "Duy", "Định", "Đạt", "Đông", "Đức", "Giang", "Hường", "Huyền", "Hạnh", "Hà", "Hiền", "Hoài", "Hằng", "Hồng", "Kiên", "Khánh", "Khương", "Linh", "Lương", "Long", "Lĩnh", "Mạnh", "Minh", "Nam", "Nguyên", "Nghĩa", "Nhật", "Phan", "Phong", "Phát", "Phi", "Phú", "Phúc", "Phước", "Phương", "Quân", "Quang", "Quốc", "Thái", "Thành", "Thắng", "Thịnh", "Thông", "Tuấn", "Tùng", "Trung", "Thanh", "Thạch", "Trí", "Tiến", "Vinh" };

                            Random rand = new Random();
                            int randHo = rand.Next(0, ho.Length);
                            int randDem = rand.Next(0, dem.Length);
                            int randTen = rand.Next(0, ten.Length);
                            string name = ho[randHo] + " " + dem[randDem] + " " + ten[randTen];

                            user.Name = name;
                            user.Account = HelperProvider.RemoveUnicode(dem[randDem]) + HelperProvider.RemoveUnicode(ten[randTen]);
                            user.Email = HelperProvider.RemoveUnicode(ho[randHo]) + HelperProvider.RemoveUnicode(dem[randDem]) + HelperProvider.RemoveUnicode(ten[randTen]) + "@gmail.com";
                            if (!userService.InsertNewUser(user, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                            UserToken userToken = new UserToken();
                            userToken.UserTokenId = Guid.NewGuid().ToString();
                            userToken.UserId = user.UserId;
                            userToken.CreateTime = HelperProvider.GetSeconds(now);
                            userToken.ExpireTime = HelperProvider.GetSeconds(now.AddDays(2));
                            if (!userService.InsertUserToken(userToken, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        }


                        transaction.Commit();
                        return Success();
                    }
                }

            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }

        }
    }
}