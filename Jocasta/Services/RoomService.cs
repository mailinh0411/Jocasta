using Dapper;
using Jocasta.Models;
using Jocasta.Providers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI.WebControls;

namespace Jocasta.Services
{
    public class RoomService : BaseService
    {
        public RoomService() : base() { }
        public RoomService(IDbConnection db) : base(db) { }

        public List<Room> GetListRoomByCategory(string category, IDbTransaction transaction = null)
        {
            string query = "select * from [room] where [RoomCategoryId] = @category";
            return this._connection.Query<Room>(query, new { category }, transaction).ToList();
        }

        public CategoryRoomAvaiable GetListRoomAvailable(string roomCategoryId, DateTime checkIn, DateTime checkOut, IDbTransaction transaction = null)
        {
            bool checkBook = false;
            CategoryRoomAvaiable categoryRoomAvaiable = new CategoryRoomAvaiable();

            string queryRoomCategory = "select * from [room_category] where RoomCategoryId = @roomCategoryId";
            categoryRoomAvaiable.Category = this._connection.Query<RoomCategory>(queryRoomCategory, new { roomCategoryId }, transaction).FirstOrDefault();

            string queryRoom = "select * from [room] where [RoomCategoryId] = @roomCategoryId and [Enable] = 1";
            List<Room> rooms = this._connection.Query<Room>(queryRoom, new { roomCategoryId }, transaction).ToList();

            categoryRoomAvaiable.ListRoom = new List<Room>();

            foreach (Room room in rooms)
            {
                if (room.Enable == false) continue;
                // Nếu từ khoảng StartDate đến EndDate có phòng đã đặt thì gán checkBook bằng true, ngược lại thì count được cộng thêm 1 
                for (DateTime dateIndex = checkIn; dateIndex < checkOut;)
                {
                    checkBook = false;
                    long date = HelperProvider.GetSeconds(dateIndex);
                    
                    string queryDayRoom =  $"select * from [day_room] where [RoomId]='{room.RoomId}' and [DayTime]=@date";
                    DayRoom dayRoom = this._connection.Query<DayRoom>(queryDayRoom, new { date }, transaction).FirstOrDefault();
                    if (dayRoom == null || (dayRoom != null && dayRoom.Status == DayRoom.EnumStatus.BOOKED))
                    {
                        checkBook = true;
                        break;
                    }
                    dateIndex = dateIndex.AddDays(1);
                }

                if (checkBook == false) categoryRoomAvaiable.ListRoom.Add(room);
            }
            return categoryRoomAvaiable;
        }
    }
}