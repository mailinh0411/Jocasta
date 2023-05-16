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
    public class AdminInvoiceController : ApiBaseAdminController
    {
        [HttpGet]
        public JsonResult GetListRoomBooked(string orderId)
        {
            try
            {
                AdminInvoiceService adminInvoiceService = new AdminInvoiceService();
                Invoice invoice = adminInvoiceService.GetInvoiceBooking(orderId);
                if (invoice == null) throw new Exception("Không có hóa đơn đặt phòng của đơn này.");

                return Success(adminInvoiceService.GetListBookingService(invoice.InvoiceId));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetListServiceBooked(string orderId)
        {
            try
            {
                AdminInvoiceService adminInvoiceService = new AdminInvoiceService();
                List<Invoice> listInvoice = adminInvoiceService.GetInvoiceService(orderId);


                List<object> listDetail = new List<object>();

                foreach (Invoice invoice in listInvoice)
                {
                    listDetail.AddRange(adminInvoiceService.GetListServiceBooked(invoice.InvoiceId));
                }

                return Success(listDetail);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        
    }
}