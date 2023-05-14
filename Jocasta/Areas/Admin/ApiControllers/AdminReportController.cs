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
        public JsonResult GetAllReportDaiy(long from, long to)
        {
            try
            {
                AdminReportService adminReportService = new AdminReportService();
                return Success(adminReportService.GetAllReportDaily(from, to));
            }catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetAllReportMonth(long from, long to)
        {
            try
            {
                AdminReportService adminReportService = new AdminReportService();
                return Success(adminReportService.GetAllReportMonth(from, to));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}