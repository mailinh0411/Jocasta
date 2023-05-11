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

        [HttpGet]
        public JsonResult GetListServiceBooked(string orderId)
        {
            try
            {
                InvoiceService invoiceService = new InvoiceService();
                List<Invoice> listInvoice = invoiceService.GetInvoiceService(orderId);
                

                List<object> listDetail = new List<object>();

                foreach (Invoice invoice in listInvoice)
                {
                    listDetail.AddRange(invoiceService.GetListServiceBooked(invoice.InvoiceId));
                }

                return Success(listDetail);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult CreateInvoiceService(CreateInvoiceService modelSerive)
        {
            try
            {
                using(var connect = BaseService.Connect())
                {
                    connect.Open();
                    using(var transaction = connect.BeginTransaction())
                    {
                        if (modelSerive.Invoices.Count <= 0) throw new Exception("Bạn cần chọn dịch vụ.");
                        OrderService orderService = new OrderService(connect);
                        InvoiceService invoiceService = new InvoiceService(connect);

                        Order order = orderService.GetOrderById(modelSerive.OrderId, transaction);
                        if (order == null) throw new Exception("Đơn đặt không tồn tại.");

                        DateTime now = DateTime.Now; 

                        // Tạo hóa đơn đặt dịch vụ của đơn đặt
                        Invoice invoice = new Invoice();
                        invoice.InvoiceId = Guid.NewGuid().ToString();
                        invoice.UserId = order.UserId;
                        invoice.OrderId = order.OrderId;
                        invoice.TotalPrice = modelSerive.TotalPrice;
                        invoice.Type = Invoice.EnumType.SERVICE_INVOICE;
                        invoice.CreateTime = HelperProvider.GetSeconds(now);
                        invoiceService.InsertInvoice(invoice, transaction);

                        foreach(ServiceOrder item in modelSerive.Invoices)
                        {
                            InvoiceDetail invoiceDetail = new InvoiceDetail();
                            invoiceDetail.InvoiceDetailId = Guid.NewGuid().ToString();
                            invoiceDetail.InvoiceId = invoice.InvoiceId;
                            invoiceDetail.ServiceId = item.ServiceId;
                            invoiceDetail.Quantity = item.Quantity;
                            invoiceDetail.Price = item.Price;

                            invoiceService.InsertInvoiceDetail(invoiceDetail, transaction);
                        }

                        // Cập nhật tổng tiền trong order
                        orderService.UpdateTotalPriceOrder(order.OrderId, modelSerive.TotalPrice, transaction);                       


                        transaction.Commit();
                        return Success();
                    }
                }
            }catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}