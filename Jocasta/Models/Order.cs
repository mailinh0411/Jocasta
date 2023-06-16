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
        public string Code { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }   
        public string Name { get; set; }
        public long CreateTime { get; set; }
        public string RequestContent { get; set; }  
        public class EnumStatus
        {
            public const string USER_CANCEL = "USER_CANCEL";
            public const string SYSTEM_CANCEL = "SYSTEM_CANCEL";
            public const string BOOKED = "BOOKED";
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
        public string UserName { get; set; }
        public string Code { get; set; }
        public long CheckIn { get; set; }
        public long CheckOut { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string RequestContent { get; set; }
    }

    public class ListOrderUserModel : BaseListModel
    {
        public List<OrderUserModel> List { get; set; }
    }

    public class CreateOrder
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string RequestContent { get; set; }
    }

    public class SystemCancelOrder
    {
        public string OrderId { get; set; }
        public string Content { get; set; }
    }
    
}