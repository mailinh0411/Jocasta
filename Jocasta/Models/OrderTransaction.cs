using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jocasta.Models
{
    public class OrderTransaction
    {
        public string OrderTransactionId { get; set; }
        public string OrderId { get; set; }
        public string UserAdminId { get; set; }
        public string Status { get; set; }
        public string Content { get; set; }
        public long CreateTime { get; set; }
    }
}