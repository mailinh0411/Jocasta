using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jocasta.Models
{
    public class ReportMonthly
    {
        public string ReportMonthlyId { get; set; }
        public decimal TotalPrice { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}