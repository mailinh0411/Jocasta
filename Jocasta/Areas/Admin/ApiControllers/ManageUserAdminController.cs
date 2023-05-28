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
    }
}