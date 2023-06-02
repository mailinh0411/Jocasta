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
        public string Type { get; set; }
        public string RequestContent { get; set; }
        public long CreateTime { get; set; }
        public class EnumType
        {
            public const string DEPOSIT_INVOICE = "DEPOSIT_INVOICE";
            public const string SERVICE_INVOICE = "SERVICE_INVOICE";
            public const string BOOKING_INVOICE = "BOOKING_INVOICE";
        }
    }

    public class ServiceOrder
    {
        public string ServiceId { get; set; }
        public string RoomId { get; set; }
        public decimal Price { get; set; }
    }

    public class CreateInvoiceService
    {
        public List<ServiceOrder> Invoices { get; set;}
        public string OrderId { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class RoomBooked
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class ServiceBooked
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public long CreateTime { get; set; }
    }

    public class ExportUserOrder
    {
        public Order OrderInfo { get; set; }
        public List<RoomBooked> ListRoomBookeds { get; set; }
        public List<ServiceBooked> ListServiceBookeds { get; set;}
    }
    
}