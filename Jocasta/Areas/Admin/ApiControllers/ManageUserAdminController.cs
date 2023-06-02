using Jocasta.Areas.Admin.Services;
using Jocasta.Models;
using Jocasta.Providers;
using Jocasta.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Jocasta.Areas.Admin.ApiControllers
{
    public class ManageUserAdminController : ApiBaseAdminController
    {
        [AllowAnonymous]
        [HttpPost]
        public JsonResult LoginAdmin(UserAdminLogin model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        if (string.IsNullOrEmpty(model.Account)) return Error("Tài khoản không được để trống.");
                        if (string.IsNullOrEmpty(model.Password)) return Error("Mật khẩu không được để trống.");

                        ManageUserAdminService adminService = new ManageUserAdminService();

                        UserAdmin admin = adminService.GetUserAdminByAccount(model.Account);
                        if (admin == null) return Error("Sai tài khoản hoặc mật khẩu.");

                        if (admin.Enable == false) return Error("Tài khoản này đã bị khóa.");

                        string password = SecurityProvider.EncodePassword(admin.UserAdminId, model.Password);
                        if (admin.Password != password) return Error("Sai tài khoản hoặc mật khẩu.");

                        string token = SecurityProvider.CreateToken(admin.UserAdminId, admin.Password, admin.Account);
                        adminService.UpdateUserAdminToken(admin.UserAdminId, token);
                        return Success(token);
                    }
                }
                
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult LogOut()
        {
            try
            {
                ManageUserAdminService userAdminService = new ManageUserAdminService();
                UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                if (userAdmin == null) return Unauthorized();
                userAdminService.UpdateUserAdminToken(userAdmin.UserAdminId, "");
                return Success();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetAdminInfo()
        {
            try
            {
                string token = Request.Headers.Authorization.ToString();
                ManageUserAdminService useradminService = new ManageUserAdminService();
                UserAdmin userAdmin = useradminService.GetUserAdminByToken(token);
                if (userAdmin == null) return Unauthorized();

                return Success(userAdmin);
            }
            catch (Exception ex)
            {

                return Error(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult ChangeAdminPassword(UserAdminChangePass model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        string token = Request.Headers.Authorization.ToString();
                        ManageUserAdminService useradminService = new ManageUserAdminService(connect);
                        UserAdmin userAdmin = useradminService.GetUserAdminByToken(token, transaction);
                        if (userAdmin == null) return Unauthorized();

                        string password = SecurityProvider.EncodePassword(userAdmin.UserAdminId, model.Password);
                        if (!userAdmin.Password.Equals(password)) return Error("Mật khẩu cũ không đúng");
                        else
                        {
                            model.NewPassword = SecurityProvider.EncodePassword(userAdmin.UserAdminId, model.NewPassword);
                            useradminService.UpdateUserAdminPassword(userAdmin.UserAdminId, model.NewPassword, transaction);

                            transaction.Commit();
                            return Success();
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Error();
            }
        }

        [HttpGet]
        public JsonResult GetListUserAdmin(string keyword, string enable, int page, int pageSize)
        {
            try
            {
                ManageUserAdminService manageUserAdminService = new ManageUserAdminService();
                return Success(manageUserAdminService.GetListUserAdmin(keyword, enable, page, pageSize));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetUserAdminById(string id)
        {
            try
            {
                ManageUserAdminService manageUserAdminService = new ManageUserAdminService();
                UserAdmin userAdmin = manageUserAdminService.GetUserAdminById(id);
                return Success(userAdmin);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult CreateUserAdmin(UserAdmin model)
        {
            try
            {
                using(var connect = BaseService.Connect())
                {
                    connect.Open();
                    using(var transaction = connect.BeginTransaction())
                    {
                        ManageUserAdminService manageUserAdminService = new ManageUserAdminService(connect);

                        if (string.IsNullOrEmpty(model.Name)) throw new Exception("Họ và tên tài khoản không được để trống.");
                        if (string.IsNullOrEmpty(model.Account) || manageUserAdminService.CheckDuplicateUser(model.Account, transaction) != null) throw new Exception("Tên đăng nhập đang trống hoặc đã tồn tại.");
                        if (string.IsNullOrEmpty(model.Email) || manageUserAdminService.CheckDuplicateUser(model.Email, transaction) != null) throw new Exception("Email đang trống hoặc đã tồn tại.");
                        if (string.IsNullOrEmpty(model.Password)) throw new Exception("Password không được để trống.");
                        if (string.IsNullOrEmpty(model.Phone) || manageUserAdminService.CheckDuplicateUser(model.Phone, transaction) != null) throw new Exception("Số điện thoại đang trống hoặc đã tồn tại.");

                        UserAdmin userAdmin = new UserAdmin();
                        userAdmin.UserAdminId = Guid.NewGuid().ToString();
                        userAdmin.Name = model.Name;
                        userAdmin.Account = model.Account;
                        userAdmin.Email = model.Email;
                        userAdmin.Phone = model.Phone;
                        userAdmin.Password = SecurityProvider.EncodePassword(userAdmin.UserAdminId, model.Password);
                        userAdmin.Enable = true;
                        if (!string.IsNullOrEmpty(model.Avatar))
                        {
                            string filename = Guid.NewGuid().ToString() + ".jpg";
                            var path = System.Web.HttpContext.Current.Server.MapPath(Constant.USER_IMAGE_PATH + filename);
                            HelperProvider.Base64ToImage(model.Avatar, path);
                            userAdmin.Avatar = Constant.USER_IMAGE_URL + filename;
                        }
                        DateTime now = DateTime.Now;
                        userAdmin.CreateTime = HelperProvider.GetSeconds(now);

                        if (!manageUserAdminService.InsertUserAdmin(userAdmin, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);                        

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
        public JsonResult UpdateUserAdmin(UserAdmin model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        ManageUserAdminService userAdminService = new ManageUserAdminService(connect);
                        UserAdmin admin = userAdminService.GetUserAdminById(model.UserAdminId, transaction);
                        if (admin == null) return Error("Tài khoản quản trị này không tồn tại.");

                        admin.Name = model.Name;
                        if (string.IsNullOrEmpty(admin.Name)) throw new Exception("Tên người dùng không được để trống.");

                        admin.Account = model.Account;
                        if (string.IsNullOrEmpty(admin.Account)) throw new Exception("Tên đăng nhập không được để trống.");
                        if (userAdminService.CheckAccountExist(admin.Account, admin.UserAdminId, transaction)) throw new Exception("Tên đăng nhập đã tồn tại.");

                        admin.Email = model.Email.Trim();
                        if (string.IsNullOrEmpty(admin.Email)) throw new Exception("Email không được để trống.");
                        if (userAdminService.CheckEmailExit(admin.Email, admin.UserAdminId, transaction)) throw new Exception("Email đã tồn tại.");
;
                        admin.Phone = model.Phone;
                        if (string.IsNullOrEmpty(admin.Phone)) throw new Exception("Số điện thoại không được để trống.");
                        if (userAdminService.CheckPhoneExit(admin.Phone, admin.UserAdminId, transaction)) throw new Exception("Số điện thoại đã tồn tại.");

                        if (!string.IsNullOrEmpty(model.Password))
                        {
                            admin.Password = SecurityProvider.EncodePassword(admin.UserAdminId, model.Password);
                        }

                        if (!string.IsNullOrEmpty(model.Avatar))
                        {
                            if (!HelperProvider.DeleteFile(admin.Avatar)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                            string filename = Guid.NewGuid().ToString() + ".jpg";
                            var path = System.Web.HttpContext.Current.Server.MapPath(Constant.USER_IMAGE_PATH + filename);
                            HelperProvider.Base64ToImage(model.Avatar, path);
                            admin.Avatar = Constant.USER_IMAGE_URL + filename;
                        }

                        if (!userAdminService.UpdateUserAdmin(admin, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);
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

        [HttpGet]
        public JsonResult ChangeUserAdminEnable(string id)
        {
            try
            {
                using(var connect = BaseService.Connect())
                {
                    connect.Open();
                    using(var transaction = connect.BeginTransaction())
                    {
                        ManageUserAdminService userAdminService = new ManageUserAdminService(connect);
                        UserAdmin userAdmin = userAdminService.GetUserAdminById(id, transaction);
                        if (userAdmin == null) throw new Exception("Tài khoản này không tồn tại.");

                        if (userAdmin.Enable == false) userAdmin.Enable = true;
                        else userAdmin.Enable = false;

                        userAdminService.ChangeUserAdminEnable(userAdmin, transaction);

                        transaction.Commit();
                        return Success();
                    }
                }
            }
            catch(Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}