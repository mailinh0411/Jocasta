using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jocasta.Models
{
    public class Cart
    {
        public string CartId { get; set; }
        public string UserId { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalPrice { get; set; }
        public long CheckIn { get; set; }
        public long CheckOut { get; set; }

    }
}