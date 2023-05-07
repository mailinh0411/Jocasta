using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jocasta.Models
{
    public class InvoiceDetail
    {
        public string InvoiceDetailId { get; set; }
        public string InvoiceId { get; set; }
        public string ServiceId { get; set; }
        public string RoomCategoryId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}