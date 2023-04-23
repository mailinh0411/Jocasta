using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jocasta.Models
{
    public class Invoice
    {
        public string InvoiceId { get; set; }
        public string UserId { get; set; }
        public string OrderId { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public long CreateTime { get; set; }
        public class EnumType
        {
            public const string DEPOSIT_INVOICE = "DEPOSIT_INVOICE";
            public const string SERVICE_INVOICE = "SERVICE_INVOICE";
            public const string BOOKING_INVOICE = "BOOKING_INVOICE";
        }
    }

    
}