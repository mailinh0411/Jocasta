using Jocasta.Models;
using Jocasta.Providers;
using Jocasta.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.UI.WebControls;

namespace Jocasta.ApiControllers
{
    public class InvoiceController : ApiBaseController
    {
        [HttpGet]
        public JsonResult GetListRoomBooked(string orderId)
        {
            try
            {
                InvoiceService invoiceService = new InvoiceService();
                Invoice invoice = invoiceService.GetInvoiceBooking(orderId);
                if (invoice == null) throw new Exception("Không có hóa đơn đặt phòng của đơn này.");

                return Success(invoiceService.GetListBookingService(invoice.InvoiceId));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}