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
        public decimal Price { get; set; }
        public int ExtraBed { get; set; }
    }

    public class OrderDetailBooking
    {
        public string OrderDetailId { get; set; }
        public string OrderId { get; set; }
        public string RoomCategoryId { get; set; }
        public int NumberOfRoom { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; }
        public string ListRoom { get; set; }
        public int ExtraBed { get; set; }
    }
}