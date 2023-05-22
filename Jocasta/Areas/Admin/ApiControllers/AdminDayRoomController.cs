using Jocasta.Areas.Admin.Services;
using Jocasta.Models;
using Jocasta.Providers;
using Jocasta.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.UI;

namespace Jocasta.Areas.Admin.ApiControllers
{
    public class AdminDayRoomController : ApiBaseAdminController
    {
        // GET: Admin/AdminDayRoom
        [HttpGet]
        public JsonResult GetListDayRoom(string keyword, string status, string startDate, string endDate, int page, int pageSize)
        {
            try
            {
                AdminDayRoomService dayRoomService = new AdminDayRoomService();
                return Success(dayRoomService.GetListDayRoom(keyword, status, startDate, endDate, page, pageSize));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetListDayRoomByDay(string keyword, string status, long startDate, long endDate, int floor)
        {
            try
            {
                AdminDayRoomService dayRoomService = new AdminDayRoomService();
                return Success(dayRoomService.GetListDayRoomByDay(keyword, status, startDate, endDate, floor));
            }
            catch(Exception ex)
            {
                return Error(ex.Message);
            }
        }


        [HttpGet]
        public JsonResult GetDayRoomById(string id)
        {
            try
            {
                AdminDayRoomService dayRoomService = new AdminDayRoomService();
                DayRoom dayRoom = dayRoomService.GetDayRoomById(id);
                return Success(dayRoomService.GetDayRoomById(id));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult InsertDayRoom(InsertDayRoom model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        AdminDayRoomService dayRoomService = new AdminDayRoomService(connect);

                        DayRoom dayRoom = new DayRoom();
                        dayRoom.DayRoomId = Guid.NewGuid().ToString();
                        dayRoom.RoomId = model.RoomId;
                        long dayTime = HelperProvider.GetSeconds(model.CreateTime);                        
                        dayRoom.DayTime = dayTime;
                        dayRoom.Status = DayRoom.EnumStatus.AVAILABLE;

                        if (!dayRoomService.InsertDayRoom(dayRoom, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);

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
        public JsonResult InsertListDayRoom(InsertListDayRoom model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        AdminDayRoomService dayRoomService = new AdminDayRoomService(connect);

                        for(int i = 0; i< model.ListRoomId.Count; i++)
                        {
                            DateTime startDate = Convert.ToDateTime(model.StartDate);
                            DateTime endDate = Convert.ToDateTime(model.EndDate);

                            /*DateTime startDate = model.StartDate;
                            DateTime endDate = model.EndDate;*/

                            while (startDate <= endDate)
                            {
                                DayRoom dayRoom = new DayRoom();
                                dayRoom.DayRoomId = Guid.NewGuid().ToString();
                                dayRoom.RoomId = model.ListRoomId[i];
                                long dayTime = HelperProvider.GetSeconds(startDate);
                                dayRoom.DayTime = dayTime;
                                dayRoom.Status = DayRoom.EnumStatus.AVAILABLE;

                                if (!dayRoomService.InsertDayRoom(dayRoom, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);

                                startDate = startDate.AddDays(1);
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
        public JsonResult UpdateDayRoom(DayRoom model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        AdminDayRoomService dayRoomService = new AdminDayRoomService(connect);

                        DayRoom dayRoom = dayRoomService.GetDayRoomById(model.RoomId, transaction);
                        if (dayRoom == null) throw new Exception("Phòng ngày này không tồn tại.");

                        dayRoom.RoomId = model.RoomId;
                        dayRoom.DayTime = model.DayTime;

                        if (!dayRoomService.UpdateDayRoom(dayRoom, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);

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
        public JsonResult DeleteDayRoom(string id)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        AdminDayRoomService dayRoomService = new AdminDayRoomService(connect);

                        DayRoom dayRoom = dayRoomService.GetDayRoomById(id, transaction);
                        if (dayRoom == null) throw new Exception("Phòng ngày này không tồn tại.");

                        if (!dayRoomService.DeleteDayRoom(id, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);
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
        public JsonResult ChangeStatusDayRoom(string id)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        AdminDayRoomService dayRoomService = new AdminDayRoomService(connect);

                        DayRoom dayRoom = dayRoomService.GetDayRoomById(id, transaction);
                        if (dayRoom == null) throw new Exception("Phòng ngày này không tồn tại.");

                        string status;
                        if (dayRoom.Status == DayRoom.EnumStatus.AVAILABLE)
                        {
                            status = DayRoom.EnumStatus.BOOKED;
                        }
                        else
                        {
                            status = DayRoom.EnumStatus.AVAILABLE;
                        }

                        if (!dayRoomService.ChangeStatusDayRoom(id, status, transaction)) return Error(JsonResult.Message.ERROR_SYSTEM);
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