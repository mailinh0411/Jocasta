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
        public JsonResult LoginPost(UserAdminLogin model)
        {
            try
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
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}