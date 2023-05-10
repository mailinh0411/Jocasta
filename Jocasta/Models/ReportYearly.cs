using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jocasta.Models
{
    public class ReportYearly
    {
        public string ReportYearlyId { get; set; }
        public decimal TotalPrice { get; set; }
        public int Year { get; set; }
    }
}