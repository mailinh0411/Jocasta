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
        public long CheckIn { get; set; }
        public long CheckOut { get; set; }
        public long CreateTime { get; set; }
        public class EnumStatus
        {
            public const string PENDING = "PENDING";
            public const string CONFIRM = "CONFIRM";
            public const string CHECKED_IN = "CHECKED_IN";
            public const string CHECKED_OUT = "CHECKED_OUT";
        }
    }

    public class OrderUserModel
    {
        public string OrderId { get; set; }
        public string UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public long CreateTime { get; set; }
        public string Name { get; set; }
        public string Account { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class ListOrderUserModel : BaseListModel
    {
        public List<OrderUserModel> List { get; set; }
    }
    
}