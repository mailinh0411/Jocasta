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
    public class OrderController : ApiBaseController
    {
        // Danh sách số lượng phòng theo từng loại phòng còn trống khi đặt phòng 
        [HttpGet]
        public JsonResult GetListRoom(string checkIn, string checkOut, string keyword)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        RoomCategoryService roomCategoryService = new RoomCategoryService(connect);
                        RoomService roomService = new RoomService(connect);
                        DayRoomService dayRoomService = new DayRoomService(connect);

                        List<CategoryCountRoom> listCategoryCountRoom = new List<CategoryCountRoom>();

                        List<RoomCategory> roomCategories = roomCategoryService.GetAllByKeyword(keyword, transaction);

                        DateTime CheckIn = Convert.ToDateTime(checkIn);
                        DateTime CheckOut = Convert.ToDateTime(checkOut);
                        int count = 0;
                        bool checkBook = false;
                        foreach (RoomCategory index in roomCategories)
                        {
                            CategoryCountRoom categoryCountRoom = new CategoryCountRoom();
                            categoryCountRoom.Category = index;
                            List<Room> rooms = roomService.GetListRoomByCategory(index.RoomCategoryId, transaction);
                            // Gán đầu tiên số lượng phòng bằng 0
                            count = 0;
                            foreach (Room room in rooms)
                            {
                                if(room.Enable == false) continue;
                                // Nếu từ khoảng StartDate đến EndDate có phòng đã đặt thì gán checkBook bằng true, ngược lại thì count được cộng thêm 1 
                                for (DateTime dateIndex = CheckIn; dateIndex <= CheckOut;)
                                {
                                    checkBook = false;
                                    long date = HelperProvider.GetSeconds(dateIndex);
                                    DayRoom dayRoom = dayRoomService.CheckDayRoomAvailable(room.RoomId, date, transaction);
                                    if(dayRoom != null && dayRoom.Status == DayRoom.EnumStatus.BOOKED)
                                    {
                                        checkBook = true;
                                        break;
                                    }
                                    dateIndex = dateIndex.AddDays(1);
                                }

                                if (checkBook == false) count += 1;
                            }

                            categoryCountRoom.Count = count;
                            listCategoryCountRoom.Add(categoryCountRoom);                            
                        }

                        return Success(listCategoryCountRoom);
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