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
    public class ManageUserController : ApiBaseAdminController
    {
        // GET: Admin/ManageUser
        [HttpGet]
        public JsonResult GetListUser(string keyword, string enable, int page, int pageSize)
        {
            try
            {
                ManageUserService manageUserService = new ManageUserService();
                return Success(manageUserService.GetListUser(keyword, enable, page, pageSize));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetUserById(string id)
        {
            try
            {
                ManageUserService manageUserService = new ManageUserService();
                User user = manageUserService.GetUserById(id);
                return Success(user);
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
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        ManageUserService manageUserService = new ManageUserService(connect);

                        User user = manageUserService.GetUserById(model.UserId, transaction);
                        if (user == null) throw new Exception("Người dùng này không tồn tại.");

                        user.Name = model.Name;
                        if (string.IsNullOrEmpty(user.Name)) throw new Exception("Tên người dùng không được để trống.");

                        user.Account = model.Account;
                        if (string.IsNullOrEmpty(user.Account)) throw new Exception("Tên đăng nhập không được để trống.");
                        if (manageUserService.CheckAccountExist(user.Account, user.UserId, transaction)) throw new Exception("Tên đăng nhập đã tồn tại.");

                        user.Email = model.Email.Trim();
                        if (manageUserService.CheckEmailExit(user.Email, user.UserId, transaction)) throw new Exception("Email đã tồn tại.");

                        user.Phone = model.Phone;

                        if (!string.IsNullOrEmpty(model.Password))
                        {
                            user.Password = SecurityProvider.EncodePassword(user.UserId, SecurityProvider.CreateMD5(SecurityProvider.Base64Encode(model.Password)));
                        }

                        if (!string.IsNullOrEmpty(model.Avatar))
                        {
                            if (!HelperProvider.DeleteFile(user.Avatar)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                            string filename = Guid.NewGuid().ToString() + ".jpg";
                            var path = System.Web.HttpContext.Current.Server.MapPath(Constant.USER_IMAGE_PATH + filename);
                            HelperProvider.Base64ToImage(model.Avatar, path);
                            user.Avatar = Constant.USER_IMAGE_URL + filename;
                        }

                        if (!manageUserService.UpdateUser(user, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

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
        public JsonResult ChangeUserEnable(string id)
        {
            try
            {
                using (var conncect = BaseService.Connect())
                {
                    conncect.Open();
                    using (var transaction = conncect.BeginTransaction())
                    {
                        ManageUserService manageUserService = new ManageUserService(conncect);

                        User user = manageUserService.GetUserById(id, transaction);
                        if (user == null) throw new Exception("Người dùng này không tồn tại.");

                        if (user.Enable == true)
                        {
                            user.Enable = false;
                        }
                        else
                        {
                            user.Enable = true;
                        }

                        if (!manageUserService.ChangeUserEnable(user, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

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
        public JsonResult DeleteUser(string id)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        ManageUserService manageUserService = new ManageUserService(connect);

                        User user = manageUserService.GetUserById(id, transaction);
                        if (user == null) throw new Exception("Người dùng này không tồn tại.");

                        if (!manageUserService.DeleteUser(id, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

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
        public JsonResult GetCountUser()
        {
            try
            {
                ManageUserService manageUserService = new ManageUserService();
                return Success(manageUserService.GetCountUser());
            }catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}