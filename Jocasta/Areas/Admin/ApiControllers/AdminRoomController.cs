using Jocasta.Models;
using Jocasta.Providers;
using Jocasta.Services;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Razor.Tokenizer.Symbols;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Jocasta.Areas.Admin.ApiControllers
{
    public class AdminRoomController : ApiBaseAdminController
    {
        // GET: Admin/AdminRoom
        [HttpGet]
        public JsonResult GetListRoom(string keyword, string roomCategory, string enable, int page, int pageSize)
        {
            try
            {
                AdminRoomService roomService = new AdminRoomService();
                return Success(roomService.GetListRoom(keyword, roomCategory, enable, page, pageSize));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetRooms()
        {
            try
            {
                AdminRoomService roomService = new AdminRoomService();
                return Success(roomService.GetRooms());
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetRoomById(string id)
        {
            try
            {
                AdminRoomService roomService = new AdminRoomService();                
                return Success(roomService.GetRoomById(id));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetListRoomByCategory(string id)
        {
            try
            {
                AdminRoomService roomService = new AdminRoomService();
                return Success(roomService.GetListRoombyCategory(id));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult InsertRoom(Room model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        AdminRoomService roomService = new AdminRoomService(connect);

                        Room room = new Room();
                        room.RoomId = Guid.NewGuid().ToString();    
                        room.Name = model.Name;
                        room.RoomCategoryId = model.RoomCategoryId;
                        room.Floor = model.Floor;
                        room.Enable = true;
                        DateTime now = DateTime.Now;
                        room.CreateTime = HelperProvider.GetSeconds(now);

                        if(!roomService.InsertRoom(room, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);

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
        public JsonResult UpdateRoom(Room model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        AdminRoomService roomService = new AdminRoomService(connect);

                        Room room = roomService.GetRoomById(model.RoomId, transaction);
                        if (room == null) throw new Exception("Phòng này không tồn tại.");

                        room.Name = model.Name;
                        room.RoomCategoryId = model.RoomCategoryId;
                        room.Floor = model.Floor;

                        if (!roomService.UpdateRoom(room, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);

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
        public JsonResult DeleteRoom(string roomId)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        AdminRoomService roomService = new AdminRoomService(connect);

                        Room room = roomService.GetRoomById(roomId, transaction);
                        if (room == null) throw new Exception("Phòng này không tồn tại.");                        

                        if (!roomService.DeleteRoom(roomId, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);
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
        public JsonResult ChangeEnableRoom(string roomId)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        AdminRoomService roomService = new AdminRoomService(connect);

                        Room room = roomService.GetRoomById(roomId, transaction);
                        if (room == null) throw new Exception("Phòng này không tồn tại.");

                        bool enable = false;
                        if(room.Enable == true)
                        {
                            enable = false;
                        }
                        else
                        {
                            enable = true;
                        }

                        if (!roomService.ChangeEnableRoom(roomId, enable, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);
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