using Jocasta.Models;
using Jocasta.Providers;
using Jocasta.Services;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Razor.Tokenizer.Symbols;
using System.Web.UI.WebControls;
using System.Web.UI;
using Jocasta.Areas.Admin.Services;


namespace Jocasta.Areas.Admin.ApiControllers
{
    public class AdminOrderController : ApiBaseAdminController
    {
        // GET: Admin/AdminRoom
        [HttpGet]
        public JsonResult GetListOrder(string keyword, int page, int pageSize)
        {
            try
            {
                AdminOrderService adminOrderService = new AdminOrderService();
                return Success(adminOrderService.GetListOrder(keyword, page, pageSize));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}