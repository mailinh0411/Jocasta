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
using System.Windows.Input;
using System.Windows.Media.TextFormatting;
using System.Windows.Media;
using System.Xml.Linq;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using iTextSharp.text.pdf;
using iTextSharp.text;

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


        private void BindingFormatForExcelOrder(ExcelWorksheet worksheet, ExportUserOrder model)
        {
            worksheet.DefaultColWidth = 10;
            worksheet.Cells.Style.WrapText = true;

            worksheet.Cells["A1:D1"].Value = "THE JOCASTA MAI LINH HOTEL";
            worksheet.Cells["A2:D2"].Value = "HÓA ĐƠN ĐẶT PHÒNG";


            worksheet.Cells["A4"].Value = "Họ và tên khách hàng: ";
            worksheet.Cells["A5"].Value = "Số điện thoại: ";
            worksheet.Cells["A6"].Value = "Email: ";
            worksheet.Cells["B4"].Value = model.OrderInfo.Name;
            worksheet.Cells["B5"].Value = model.OrderInfo.Phone;
            worksheet.Cells["B6"].Value = model.OrderInfo.Email;

            DateTime checkIn = HelperProvider.GetDateTime(model.OrderInfo.CheckIn);
            DateTime checkOut = HelperProvider.GetDateTime(model.OrderInfo.CheckOut);

            worksheet.Cells["D4"].Value = "Mã đơn đặt: ";
            worksheet.Cells["D5"].Value = "Ngày check in: ";
            worksheet.Cells["D6"].Value = "Ngày check out: ";
            worksheet.Cells["E4"].Value = model.OrderInfo.Code;
            worksheet.Cells["E5"].Value = checkIn;
            worksheet.Cells["E6"].Value = checkOut;

            worksheet.Cells["A7"].Value = "STT: ";
            worksheet.Cells["B7"].Value = "Loại phòng: ";
            worksheet.Cells["C7"].Value = "Số lượng: ";
            worksheet.Cells["D7"].Value = "Giá phòng: ";

            using (var range = worksheet.Cells["A7:D7"])
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 221, 177));
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.Font.SetFromFont("Arial", 10);
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                range.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.WhiteSmoke);
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.Font.Bold = true;
            }
            using (var range = worksheet.Cells["A8:D8"])
            {
                range.Style.Font.Bold = true;
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
            for (int i = 0; i < model.ListRoomBookeds.Count; i++)
            {
                worksheet.Cells["A" + (i + 8)].Value = i + 1;
            }
            for (int i = 0; i < model.ListRoomBookeds.Count; i++)
            {
                var item = model.ListRoomBookeds[i];
                worksheet.Cells["B" + (i + 8)].Value = item.Name;
                worksheet.Cells["C" + (i + 8)].Value = item.Quantity;
                worksheet.Cells["D" + (i + 8)].Value = item.Price;
                using (var range = worksheet.Cells["A" + (i + 8) + ":D" + i + 8])
                {
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }
                worksheet.Row(i + 8).Height = 30;
            }

            using (var range = worksheet.Cells["A" + (model.ListRoomBookeds.Count + 1) + ":D" + model.ListRoomBookeds.Count + 1])
            {
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Row(model.ListRoomBookeds.Count + 1).Height = 30;
            }
            for (int i = 1; i <= worksheet.Dimension.End.Column; i++) { worksheet.Column(i).AutoFit(); }
            worksheet.Cells[1, 1, model.ListRoomBookeds.Count + 10, 9].AutoFitColumns(20);
            worksheet.Cells["A8:A" + model.ListRoomBookeds.Count + 8].AutoFitColumns(10);
            worksheet.Cells["D8:D" + model.ListRoomBookeds.Count + 8].AutoFitColumns(40);
            worksheet.Cells["D7:D7"].AutoFilter = true;

            int currentRoomLength = model.ListRoomBookeds.Count;

            if (model.ListServiceBookeds.Count > 0)
            {
                worksheet.Cells["A" + (currentRoomLength + 8 + 1)].Value = "STT: ";
                worksheet.Cells["B" + (currentRoomLength + 8 + 1)].Value = "Dịch vụ: ";
                worksheet.Cells["C" + (currentRoomLength + 8 + 1)].Value = "Số lượng: ";
                worksheet.Cells["D" + (currentRoomLength + 8 + 1)].Value = "Giá phòng: ";
                worksheet.Cells["E" + (currentRoomLength + 8 + 1)].Value = "Ngày đặt: ";

                using (var range = worksheet.Cells["A" + (currentRoomLength + 8 + 1) + ":E" + (currentRoomLength + 8 + 1)])
                {
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 221, 177));
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.Font.SetFromFont("Arial", 10);
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    range.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.WhiteSmoke);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    range.Style.Font.Bold = true;
                }
                using (var range = worksheet.Cells["A" + (currentRoomLength + 8 + 2) + ":E" + (currentRoomLength + 8 + 2)])
                {
                    range.Style.Font.Bold = true;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }
                for (int i = 0; i < model.ListServiceBookeds.Count; i++)
                {
                    worksheet.Cells["A" + (i + currentRoomLength + 8 + 2)].Value = i + 1;
                }
                for (int i = 0; i < model.ListServiceBookeds.Count; i++)
                {
                    var item = model.ListServiceBookeds[i];
                    DateTime ngaydat = HelperProvider.GetDateTime(item.CreateTime);
                    worksheet.Cells["B" + (i + currentRoomLength + 8 + 2)].Value = item.Name;
                    worksheet.Cells["C" + (i + currentRoomLength + 8 + 2)].Value = item.Quantity;
                    worksheet.Cells["D" + (i + currentRoomLength + 8 + 2)].Value = item.Price;
                    worksheet.Cells["E" + (i + currentRoomLength + 8 + 2)].Value = ngaydat;
                    using (var range = worksheet.Cells["A" + (i + currentRoomLength + 8 + 2) + ":E" + (i + currentRoomLength + 8 + 2)])
                    {
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }
                    worksheet.Row(i + currentRoomLength + 8 + 2).Height = 30;
                }
            }

        }

        private Stream CreateExcelFileOrder(ExportUserOrder model, Stream stream = null)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                excelPackage.Workbook.Properties.Author = "MaiLinhHotel";
                excelPackage.Workbook.Properties.Title = "Create Excel File";
                excelPackage.Workbook.Properties.Comments = "Hóa đơn mã " + model.OrderInfo.Code;
                excelPackage.Workbook.Worksheets.Add("Trang 1");
                var workSheet = excelPackage.Workbook.Worksheets[0];
                BindingFormatForExcelOrder(workSheet, model);

                excelPackage.Save();
                return excelPackage.Stream;
            }
        }
        [HttpPost]
        public HttpResponseMessage ExportFileExcelOrder(ExportUserOrder model)
        {
            using (var streams = CreateExcelFileOrder(model, null) as MemoryStream)
            {
                using (var memoryStream = new MemoryStream())
                {
                    var nameFile = "";
                    streams.Position = 0;
                    streams.CopyTo(memoryStream);
                    var result = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(memoryStream.ToArray())
                    };
                    nameFile = "HoaDonMa" + model.OrderInfo.Code;
                    result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = nameFile + ".xlsx"
                    };
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    return result;
                }
            }
        }

        [HttpPost]
        public HttpResponseMessage ExportPdf(ExportUserOrder model)
        {
            // Tạo một đối tượng Document mới
            Document document = new Document();
            MemoryStream stream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, stream);

            // Mở tài liệu
            document.Open();

            // Thêm các thông tin vào tài liệu, ví dụ như tiêu đề, dữ liệu hóa đơn, vv.
            document.Add(new Paragraph("Invoice Details"));
            document.Add(new Paragraph("Invoice Number: 001"));
            document.Add(new Paragraph("Date: 2023-05-22"));
            document.Add(new Paragraph("Customer Name: John Doe"));
            document.Add(new Paragraph(""));

            // Tạo một bảng để hiển thị các mặt hàng hóa đơn
            PdfPTable table = new PdfPTable(3);
            table.AddCell("Product");
            table.AddCell("Quantity");
            table.AddCell("Price");
            table.AddCell("Product A");
            table.AddCell("1");
            table.AddCell("100.00");
            table.AddCell("Product B");
            table.AddCell("2");
            table.AddCell("50.00");
            table.AddCell("Product C");
            table.AddCell("3");
            table.AddCell("30.00");
            document.Add(table);

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
