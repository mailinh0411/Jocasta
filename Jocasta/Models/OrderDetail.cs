using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jocasta.Models
{
    public class OrderDetail
    {
        public string OrderDetailId { get; set; }
        public string OrderId { get; set; }
        public string RoomCategoryId { get; set; }
        public int NumberOfRoom { get; set; }
        public long CheckIn { get; set; }
        public long CheckOut { get; set; }
    }
}