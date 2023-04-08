using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jocasta.Models
{
    public class Room
    {
        public string RoomId { get; set; }
        public string Name { get; set; }
        public int Floor { get; set; }
        public string RoomCategoryId { get; set; }
        public bool Enable { get; set; }
        public long CreateTime { get; set; }
        public string RoomCategoryName { get; set; }
    }

    public class ListRoomView : BaseListModel
    {
        public List<Room> List { get; set; }
    }
}