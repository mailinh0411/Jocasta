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
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using iTextSharp.text;
using iTextSharp.text.pdf;

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


        [HttpPost]
        public HttpResponseMessage ExportPdf(ExportUserOrder model)
        {
            UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
            if (userAdmin == null) throw new Exception("Người dùng này không tồn tại.");

            BaseFont font = BaseFont.CreateFont(@"C:\Windows\Fonts\Arial.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            iTextSharp.text.Font vietnameseFont = new iTextSharp.text.Font(font, 12, iTextSharp.text.Font.NORMAL);


            BaseFont fontContent = BaseFont.CreateFont(@"C:\Windows\Fonts\Arial.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            iTextSharp.text.Font vietnameseFontContent = new iTextSharp.text.Font(font, 16, iTextSharp.text.Font.NORMAL);

            // Tạo một đối tượng Document mới
            Document document = new Document();
            MemoryStream stream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, stream);

            // Mở tài liệu
            document.Open();

            // Create a PdfContentByte object
            PdfContentByte cb = writer.DirectContent;

            // Add content to the document
            PdfPCell cell = new PdfPCell(new Paragraph("HÓA ĐƠN ĐẶT PHÒNG", vietnameseFontContent));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderWidth = 0;
            PdfPTable table = new PdfPTable(1);
            table.AddCell(cell);
            document.Add(table);


            // Add order info
            PdfPTable tableOrderInfo = new PdfPTable(2);
            PdfPCell cellOrderInfo = new PdfPCell();
            cellOrderInfo = new PdfPCell(new Paragraph("Họ và tên khách hàng: " + model.OrderInfo.Name, vietnameseFont));
            cellOrderInfo.BorderWidth = 0;
            cellOrderInfo.PaddingTop = 10f;
            cellOrderInfo.PaddingBottom = 10f;
            tableOrderInfo.AddCell(cellOrderInfo);

            cellOrderInfo = new PdfPCell(new Paragraph("Mã đơn đặt: " + model.OrderInfo.Code, vietnameseFont));
            cellOrderInfo.BorderWidth = 0;
            cellOrderInfo.PaddingTop = 10f;
            cellOrderInfo.PaddingBottom = 10f;
            tableOrderInfo.AddCell(cellOrderInfo);

            cellOrderInfo = new PdfPCell(new Paragraph("Số điện thoại: " + model.OrderInfo.Phone, vietnameseFont));
            cellOrderInfo.BorderWidth = 0;
            cellOrderInfo.PaddingTop = 10f;
            cellOrderInfo.PaddingBottom = 10f;
            tableOrderInfo.AddCell(cellOrderInfo);

            DateTime checkIn = HelperProvider.GetDateTime(model.OrderInfo.CheckIn);
            string from = (checkIn.Day > 10 ? checkIn.Day.ToString() :  "0" + checkIn.Day) + '/' + (checkIn.Month > 10 ? checkIn.Month.ToString() : "0" + checkIn.Month) + '/' + checkIn.Year.ToString();    
            cellOrderInfo = new PdfPCell(new Paragraph("Ngày đến: " + from, vietnameseFont));
            cellOrderInfo.BorderWidth = 0;
            cellOrderInfo.PaddingTop = 10f;
            cellOrderInfo.PaddingBottom = 10f;
            tableOrderInfo.AddCell(cellOrderInfo);

            cellOrderInfo = new PdfPCell(new Paragraph("Email: " + model.OrderInfo.Email, vietnameseFont));
            cellOrderInfo.BorderWidth = 0;
            cellOrderInfo.PaddingTop = 10f;
            cellOrderInfo.PaddingBottom = 10f;
            tableOrderInfo.AddCell(cellOrderInfo);

            DateTime checkOut = HelperProvider.GetDateTime(model.OrderInfo.CheckOut);
            string to = (checkOut.Day > 10 ? checkOut.Day.ToString() : "0" + checkOut.Day) + '/' + (checkOut.Month > 10 ? checkOut.Month.ToString() : "0" + checkOut.Month) + '/' + checkOut.Year.ToString();
            cellOrderInfo = new PdfPCell(new Paragraph("Ngày đi: " + to, vietnameseFont));
            cellOrderInfo.BorderWidth = 0;
            cellOrderInfo.PaddingTop = 10f;
            cellOrderInfo.PaddingBottom = 10f;
            tableOrderInfo.AddCell(cellOrderInfo);

            tableOrderInfo.SpacingBefore = 20f;
            tableOrderInfo.SpacingAfter = 10f;
            tableOrderInfo.WidthPercentage = 100f;
            tableOrderInfo.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            document.Add(tableOrderInfo);


            // Table Room Category
            PdfPTable tableRoom = new PdfPTable(4);
            float[] widths = new float[] { 1f, 2f, 1f, 1f };
            tableRoom.SetWidths(widths);
            PdfPCell cellRoom = new PdfPCell();
            cellRoom = new PdfPCell(new Paragraph("STT", vietnameseFont));
            cellRoom.HorizontalAlignment = Element.ALIGN_CENTER;
            cellRoom.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellRoom.PaddingTop = 10f;
            cellRoom.PaddingBottom = 10f;
            tableRoom.AddCell(cellRoom);

            cellRoom = new PdfPCell(new Paragraph("Loại phòng", vietnameseFont));
            cellRoom.HorizontalAlignment = Element.ALIGN_CENTER;
            cellRoom.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellRoom.PaddingTop = 10f;
            cellRoom.PaddingBottom = 10f;
            tableRoom.AddCell(cellRoom);

            cellRoom = new PdfPCell(new Paragraph("Số lượng", vietnameseFont));
            cellRoom.HorizontalAlignment = Element.ALIGN_CENTER;
            cellRoom.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellRoom.PaddingTop = 10f;
            cellRoom.PaddingBottom = 10f;
            tableRoom.AddCell(cellRoom);

            cellRoom = new PdfPCell(new Paragraph("Giá", vietnameseFont));
            cellRoom.HorizontalAlignment = Element.ALIGN_CENTER;
            cellRoom.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellRoom.PaddingTop = 10f;
            cellRoom.PaddingBottom = 10f;
            tableRoom.AddCell(cellRoom);

            for (int i = 0; i < model.ListRoomBookeds.Count; i++)
            {
                var item = model.ListRoomBookeds[i];
                cellRoom = new PdfPCell(new Paragraph("" + (i+1)));
                cellRoom.HorizontalAlignment = Element.ALIGN_CENTER;
                cellRoom.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellRoom.PaddingTop = 10f;
                cellRoom.PaddingBottom = 10f;
                tableRoom.AddCell(cellRoom);

                cellRoom = new PdfPCell(new Paragraph(""+item.Name, vietnameseFont));
                cellRoom.HorizontalAlignment = Element.ALIGN_CENTER;
                cellRoom.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellRoom.PaddingTop = 10f;
                cellRoom.PaddingBottom = 10f;
                tableRoom.AddCell(cellRoom);

                cellRoom = new PdfPCell(new Paragraph(""+item.Quantity, vietnameseFont));
                cellRoom.HorizontalAlignment = Element.ALIGN_CENTER;
                cellRoom.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellRoom.PaddingTop = 10f;
                cellRoom.PaddingBottom = 10f;
                tableRoom.AddCell(cellRoom);

                cellRoom = new PdfPCell(new Paragraph(""+item.Price, vietnameseFont));
                cellRoom.HorizontalAlignment = Element.ALIGN_CENTER;
                cellRoom.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellRoom.PaddingTop = 10f;
                cellRoom.PaddingBottom = 10f;
                tableRoom.AddCell(cellRoom);
            }


            tableRoom.SpacingBefore = 20f;
            tableRoom.SpacingAfter = 10f;
            tableRoom.WidthPercentage = 100f;
            tableRoom.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            document.Add(tableRoom);

            // Table dịch vụ
            if(model.ListServiceBookeds.Count > 0)
            {
                PdfPTable tableService = new PdfPTable(4);
                widths = new float[] { 1f, 2f, 1f, 1f };
                tableService.SetWidths(widths);
                PdfPCell cellService = new PdfPCell();
                cellService = new PdfPCell(new Paragraph("STT", vietnameseFont));
                cellService.HorizontalAlignment = Element.ALIGN_CENTER;
                cellService.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellService.PaddingTop = 10f;
                cellService.PaddingBottom = 10f;
                tableService.AddCell(cellService);

                cellService = new PdfPCell(new Paragraph("Dịch vụ", vietnameseFont));
                cellService.HorizontalAlignment = Element.ALIGN_CENTER;
                cellService.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellService.PaddingTop = 10f;
                cellService.PaddingBottom = 10f;
                tableService.AddCell(cellService);

                cellService = new PdfPCell(new Paragraph("Số lượng", vietnameseFont));
                cellService.HorizontalAlignment = Element.ALIGN_CENTER;
                cellService.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellService.PaddingTop = 10f;
                cellService.PaddingBottom = 10f;
                tableService.AddCell(cellService);

                cellService = new PdfPCell(new Paragraph("Giá", vietnameseFont));
                cellService.HorizontalAlignment = Element.ALIGN_CENTER;
                cellService.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellService.PaddingTop = 10f;
                cellService.PaddingBottom = 10f;
                tableService.AddCell(cellService);

                for (int i = 0; i < model.ListServiceBookeds.Count; i++)
                {
                    var item = model.ListServiceBookeds[i];
                    cellService = new PdfPCell(new Paragraph("" +(i + 1)));
                    cellService.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellService.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cellService.PaddingTop = 10f;
                    cellService.PaddingBottom = 10f;
                    tableService.AddCell(cellService);

                    cellService = new PdfPCell(new Paragraph("" + item.Name, vietnameseFont));
                    cellService.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellService.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cellService.PaddingTop = 10f;
                    cellService.PaddingBottom = 10f;
                    tableService.AddCell(cellService);

                    cellService = new PdfPCell(new Paragraph("" + item.Quantity, vietnameseFont));
                    cellService.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellService.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cellService.PaddingTop = 10f;
                    cellService.PaddingBottom = 10f;
                    tableService.AddCell(cellService);

                    cellService = new PdfPCell(new Paragraph("" + item.Price, vietnameseFont));
                    cellService.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellService.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cellService.PaddingTop = 10f;
                    cellService.PaddingBottom = 10f;
                    tableService.AddCell(cellService);
                }


                tableService.SpacingBefore = 20f;
                tableService.SpacingAfter = 10f;
                tableService.WidthPercentage = 100f;
                tableService.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                document.Add(tableService);
            }

            // Người lập hóa đơn
            PdfPTable table1 = new PdfPTable(1);
            table1.SpacingBefore = 10f;
            table1.SpacingAfter = 10f;

            PdfPCell cell1 = new PdfPCell(new Paragraph("Tổng tiền: " + model.OrderInfo.TotalPrice + " VNĐ", vietnameseFont));
            cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell1.BorderWidth = 0;
            table1.AddCell(cell1);

            cell1 = new PdfPCell(new Paragraph("Người lập hóa đơn", vietnameseFont));
            cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell1.BorderWidth = 0;
            cell1.PaddingTop = 30f;
            table1.AddCell(cell1);

            cell1 = new PdfPCell(new Paragraph(userAdmin.Name, vietnameseFont));
            cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell1.BorderWidth = 0;
            cell1.PaddingTop = 30f;
            cell1.PaddingRight = 10f;
            table1.AddCell(cell1);

            document.Add(table1);

            // Đóng tài liệu
            document.Close();

            // Trả về tệp PDF dưới dạng phản hồi HTTP
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(stream.ToArray());
            response.Content.Headers.Add("Content-Type", "application/pdf");
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = "invoice.pdf";
            return response;
        }
    }

}
