using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jocasta.Models
{
    public class ReportDaily
    {
        public string ReportDailyId { get; set; }
        public decimal TotalPrice { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }

    public class ReportData
    {
        public string Time { get; set; }
        public decimal Price { get; set; }
    }


    public class ReportModel
    {
        public List<ReportData> ListReportData { get; set; }
        public decimal Revenue { get; set; }
        public string TypeReport { get; set; }
    }
}