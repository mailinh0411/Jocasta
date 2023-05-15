using Jocasta.Areas.Admin.Services;
using Jocasta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Jocasta.Areas.Admin.ApiControllers
{
    public class AdminReportController : ApiBaseAdminController
    {
        [HttpGet]
        public JsonResult GetAllReportMonthYear()
        {
            try
            {
                AdminReportService adminReportService = new AdminReportService();

                DateTime now = DateTime.Now;
                DateTime prevmonth = now.AddMonths(-1);
                DateTime prevyear = now.AddYears(-1);

                // Báo cáo doanh thu tháng này, tháng trước
                object ListReportThisMonth = adminReportService.GetListReportDaily(now.Month, now.Year);
                object ListReportPrevMonth = adminReportService.GetListReportDaily(prevmonth.Month, prevmonth.Year);

                // Báo cáo doanh thu năm này, năm trước
                object ListReportThisYear = adminReportService.GetListReportMonth(now.Year);
                object ListReportPrevYear = adminReportService.GetListReportMonth(prevyear.Year);

                return Success(new {ListReportThisMonth, ListReportPrevMonth, ListReportThisYear, ListReportPrevYear});
                
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetReportByAboutTime(long from, long to)
        {
            try
            {
                AdminReportService adminReportService = new AdminReportService();
                return Success(adminReportService.GetReportByAboutTime(from, to));
            }catch(Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}