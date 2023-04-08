using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jocasta.Models
{
    public class DayRoom
    {
        public string DayRoomId { get; set; }
        public string RoomId { get; set; }
        public string OrderDetailId { get; set; }
        public string Status { get; set; }
        public long DayTime { get; set; }
        public class EnumStatus
        {
            public const string AVAILABLE = "AVAILABLE";
            public const string BOOKED = "BOOKED";
        }
    }

    public class InsertDayRoom
    {
        public string RoomId { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class InsertListDayRoom
    {
        public List<string> ListRoomId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }

    public class DayRoomModel
    {
        public string DayRoomId { get; set; }
        public string RoomId { get; set; }
        public string OrderDetailId { get; set; }
        public string Status { get; set; }
        public long DayTime { get; set; }
        public string RoomName { get; set; }
    }

    public class ListDayRoomView : BaseListModel
    {
        public List<DayRoomModel> List { get; set; }
    }

    
}