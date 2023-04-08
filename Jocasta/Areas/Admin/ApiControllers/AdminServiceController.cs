using Jocasta.Models;
using Jocasta.Providers;
using Jocasta.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Jocasta.Areas.Admin.ApiControllers
{
    public class AdminServiceController : ApiBaseAdminController
    {
        // GET: Admin/AdminService
        [HttpGet]
        public JsonResult GetListService(string keyword, int page, int pageSize)
        {
            try
            {
                AdminServiceRoomService serviceRoomService = new AdminServiceRoomService();
                return Success(serviceRoomService.GetListService(keyword, page, pageSize));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetServiceById(string id)
        {
            try
            {
                AdminServiceRoomService serviceRoomService = new AdminServiceRoomService();
                return Success(serviceRoomService.GetServiceById(id));
            }catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult InsertService(Service model)
        {
            try
            {
                using(var connect = BaseService.Connect())
                {
                    connect.Open();
                    using(var trasaction =  connect.BeginTransaction())
                    {
                        AdminServiceRoomService serviceRoomService = new AdminServiceRoomService(connect);

                        Service service = new Service();
                        service.ServiceId = Guid.NewGuid().ToString();
                        service.Name = model.Name;
                        service.Price = model.Price;
                        service.Description = model.Description;
                        service.Enable = true;
                        if (!string.IsNullOrEmpty(model.Image))
                        {
                            string filename = Guid.NewGuid().ToString() + ".jpg";
                            var path = System.Web.HttpContext.Current.Server.MapPath(Constant.SERVICE_IMAGE_PATH + filename);
                            HelperProvider.Base64ToImage(model.Image, path);
                            service.Image = Constant.SERVICE_IMAGE_URL + filename;
                        }
                        DateTime now= DateTime.Now;
                        service.CreateTime = HelperProvider.GetSeconds(now);    

                        if(!serviceRoomService.InsertService(service, trasaction)) return Error(JsonResult.Message.ERROR_SYSTEM);

                        trasaction.Commit();
                        return Success();
                    }
                }
            }catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult UpdateService(Service model)
        {
            try
            {
                using(var connect = BaseService.Connect())
                {
                    connect.Open();
                    using(var transaction  = connect.BeginTransaction())
                    {
                        AdminServiceRoomService serviceRoomService = new AdminServiceRoomService(connect);

                        Service service = serviceRoomService.GetServiceById(model.ServiceId, transaction);
                        if(service == null) throw new Exception("Dịch vụ này không tồn tại.");

                        service.Name = model.Name;
                        service.Price = model.Price;
                        service.Description = model.Description;
                        if (!string.IsNullOrEmpty(model.Image))
                        {
                            if (!HelperProvider.DeleteFile(service.Image)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                            string filename = Guid.NewGuid().ToString() + ".jpg";
                            var path = System.Web.HttpContext.Current.Server.MapPath(Constant.SERVICE_IMAGE_PATH + filename);
                            HelperProvider.Base64ToImage(model.Image, path);
                            service.Image = Constant.SERVICE_IMAGE_URL + filename;
                        }

                        if(!serviceRoomService.UpdateService(service, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);

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
        public JsonResult DeleteService(string id)
        {
            try
            {
                using(var connect = BaseService.Connect())
                {
                    connect.Open(); 
                    using(var transaction = connect.BeginTransaction())
                    {
                        AdminServiceRoomService serviceRoomService = new AdminServiceRoomService(connect);

                        Service service = serviceRoomService.GetServiceById(id, transaction);
                        if(service == null) throw new Exception("Dịch vụ này không tồn tại.");

                        if (!string.IsNullOrEmpty(service.Image))
                        {
                            if (!HelperProvider.DeleteFile(service.Image)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                        }
                        if (!serviceRoomService.DeleteService(id, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        transaction.Commit();
                        return Success();
                    }
                }
            }catch(Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}