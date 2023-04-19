using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jocasta.Models
{
    public class Order
    {
        public string OrderId { get; set; }
        public string UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public long CreateTime { get; set; }
        public class EnumStatus
        {
            public const string PENDING = "PENDING";
            public const string CONFIRM = "BOOKED";
            public const string CHECKED_IN = "CHECKED_IN";
            public const string CHECKED_OUT = "CHECKED_OUT";
        }
    }

    
}