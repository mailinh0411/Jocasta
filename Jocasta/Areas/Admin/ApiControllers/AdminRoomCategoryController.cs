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
    public class AdminRoomCategoryController : ApiBaseAdminController
    {
        // GET: Admin/RoomCategory
        [HttpGet]
        public JsonResult GetListRoomCategory(string keyword, int? page, int? pageSize)
        {
            try
            {
                AdminRoomCategoryService roomCategoryService = new AdminRoomCategoryService();
                if(!page.HasValue) { page = 0; }
                if(!pageSize.HasValue) { pageSize = 25; }
                return Success(roomCategoryService.GetListRoomCategory(keyword, page.Value, pageSize.Value));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetRoomCategories(string keyword)
        {
            try
            {
                AdminRoomCategoryService roomCategoryService = new AdminRoomCategoryService();
                if (!string.IsNullOrEmpty(keyword)){
                    keyword = HelperProvider.RemoveUnicode(keyword);
                }
                
                return Success(roomCategoryService.GetRoomCategories(keyword));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetRoomCategoryDetail(string roomCategoryId)
        {
            try
            {
                AdminRoomCategoryService roomCategoryService = new AdminRoomCategoryService();
                RoomCategory roomCategory = roomCategoryService.GetRoomCategoryById(roomCategoryId);
                List<ImageRoom> roomImages = roomCategoryService.GetListImageRoom(roomCategoryId);
                return Success(new { roomCategory, roomImages });
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult InsertRoomCategory(RoomCategoryModel model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        AdminRoomCategoryService roomCategoryService = new AdminRoomCategoryService(connect);

                        RoomCategory roomCategory = new RoomCategory();
                        roomCategory.RoomCategoryId = Guid.NewGuid().ToString();
                        roomCategory.Name = model.Name;
                        roomCategory.View = model.View;
                        roomCategory.Acreage = model.Acreage;
                        roomCategory.NumberOfPeople = model.NumberOfPeople;
                        roomCategory.SingleBed = model.SingleBed;
                        roomCategory.DoubleBed = model.DoubleBed;
                        roomCategory.Price = model.Price;
                        roomCategory.Description = model.Description;
                        roomCategory.Enable = true;
                        DateTime now = DateTime.Now;
                        roomCategory.CreateTime = HelperProvider.GetSeconds(now);
                        roomCategory.SearchName = HelperProvider.RemoveUnicode(model.Name);
                        if (!string.IsNullOrEmpty(model.Image))
                        {
                            string filename = Guid.NewGuid().ToString() + ".jpg";
                            var path = System.Web.HttpContext.Current.Server.MapPath(Constant.ROOM_CATEGORY_IMAGE_PATH + filename);
                            HelperProvider.Base64ToImage(model.Image, path);
                            roomCategory.Image = Constant.ROOM_CATEGORY_IMAGE_URL + filename;
                        }
                        if (!roomCategoryService.InsertRoomCategory(roomCategory, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);

                        if (model.ListImage.Length > 0)
                        {
                            foreach (string imageBase64 in model.ListImage)
                            {
                                string filename = Guid.NewGuid().ToString() + ".jpg";
                                var path = System.Web.HttpContext.Current.Server.MapPath(Constant.LIST_ROOM_CATEGORY_IMAGE_PATH + filename);
                                HelperProvider.Base64ToImage(imageBase64, path);

                                ImageRoom imageRoom = new ImageRoom();
                                imageRoom.ImageRoomId = Guid.NewGuid().ToString();
                                imageRoom.RoomCategoryId = roomCategory.RoomCategoryId;
                                imageRoom.LinkImage = Constant.LIST_ROOM_CATEGORY_IMAGE_URL + filename;
                                if (!roomCategoryService.InsertImageRoom(imageRoom, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);
                            }
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

        [HttpPost]
        public JsonResult UpdateRoomCategory(RoomCategoryModel model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        AdminRoomCategoryService roomCategoryService = new AdminRoomCategoryService(connect);
                        RoomCategory roomCategory = roomCategoryService.GetRoomCategoryById(model.RoomCategoryId, transaction);
                        if (roomCategory == null) throw new Exception("Loại phòng này không tồn tại.");

                        roomCategory.Name = model.Name;
                        roomCategory.View = model.View;
                        roomCategory.Acreage = model.Acreage;
                        roomCategory.NumberOfPeople = model.NumberOfPeople;
                        roomCategory.SingleBed = model.SingleBed;
                        roomCategory.DoubleBed = model.DoubleBed;
                        roomCategory.Price = model.Price;
                        roomCategory.Description = model.Description;
                        roomCategory.SearchName = HelperProvider.RemoveUnicode(model.Name);
                        if (!string.IsNullOrEmpty(model.Image))
                        {
                            //xóa file cũ
                            if (!HelperProvider.DeleteFile(roomCategory.Image)) return Error(JsonResult.Message.ERROR_SYSTEM);
                            //tạo file mới
                            string filename = Guid.NewGuid().ToString() + ".jpg";
                            var path = System.Web.HttpContext.Current.Server.MapPath(Constant.ROOM_CATEGORY_IMAGE_PATH + filename);
                            HelperProvider.Base64ToImage(model.Image, path);
                            roomCategory.Image = Constant.ROOM_CATEGORY_IMAGE_URL + filename;
                        }
                        if (!roomCategoryService.UpdateRoomCategory(roomCategory, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);

                        //thêm danh sách ảnh
                        if (model.ListImage.Length > 0)
                        {
                            foreach (string imageBase64 in model.ListImage)
                            {
                                string filename = Guid.NewGuid().ToString() + ".jpg";
                                var path = System.Web.HttpContext.Current.Server.MapPath(Constant.LIST_ROOM_CATEGORY_IMAGE_PATH + filename);
                                HelperProvider.Base64ToImage(imageBase64, path);

                                ImageRoom imageRoom = new ImageRoom();
                                imageRoom.ImageRoomId = Guid.NewGuid().ToString();
                                imageRoom.RoomCategoryId = roomCategory.RoomCategoryId;
                                imageRoom.LinkImage = Constant.LIST_ROOM_CATEGORY_IMAGE_URL + filename;
                                if (!roomCategoryService.InsertImageRoom(imageRoom, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);
                            }
                        }
                        //xóa danh sách ảnh cũ
                        if (model.ListImageDelete.Length > 0)
                        {
                            foreach (string imageDelete in model.ListImageDelete)
                            {
                                ImageRoom imageRoom = roomCategoryService.GetImageRoomById(imageDelete, transaction);
                                if (imageRoom == null) continue;
                                if (!roomCategoryService.DeleteImageRoom(imageDelete, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);
                                if (!HelperProvider.DeleteFile(imageRoom.LinkImage)) return Error(JsonResult.Message.ERROR_SYSTEM);
                            }
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

        [HttpGet]
        public JsonResult DeleteRoomCategory(string roomCategoryId)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        AdminRoomCategoryService roomCategoryService = new AdminRoomCategoryService(connect);
                        RoomCategory roomCategory = roomCategoryService.GetRoomCategoryById(roomCategoryId, transaction);
                        if (roomCategory == null) return Error("Loại phòng này không tồn tại.");

                        //xoá ảnh đại diện
                        if (!string.IsNullOrEmpty(roomCategory.Image))
                        {
                            if (!HelperProvider.DeleteFile(roomCategory.Image)) return Error(JsonResult.Message.ERROR_SYSTEM);
                        }

                        //xóa danh sách ảnh
                        List<ImageRoom> listRoomImage = roomCategoryService.GetListImageRoom(roomCategoryId, transaction);
                        if (listRoomImage.Count > 0)
                        {
                            foreach (ImageRoom imageDelete in listRoomImage)
                            {
                                if (!roomCategoryService.DeleteImageRoom(imageDelete.ImageRoomId, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);
                                if (!HelperProvider.DeleteFile(imageDelete.LinkImage)) return Error(JsonResult.Message.ERROR_SYSTEM);
                            }
                        }
                        if (!roomCategoryService.DeleteRoomCategory(roomCategoryId, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);
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