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


        private void BindingFormatForPdfOrder(ExportUserOrder model)
        {
            // Khởi tạo đối tượng PDF
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Thêm nội dung tài liệu PDF
            XFont font = new XFont("Verdana", 20, XFontStyle.Bold);
            gfx.DrawString("HÓA ĐƠN", font, XBrushes.Black, new XRect(0, 0, page.Width.Point, 50), XStringFormats.Center);

            // Thêm thông tin hóa đơn vào tài liệu PDF
            XTextFormatter tf = new XTextFormatter(gfx);
            tf.DrawString($"Mã hóa đơn: {model.OrderInfo.Code}", font, XBrushes.Black, new XRect(50, 100, page.Width.Point, 50), XStringFormats.TopLeft);
            tf.DrawString($"Ngày xuất hóa đơn: {DateTime.Now}", font, XBrushes.Black, new XRect(50, 130, page.Width.Point, 50), XStringFormats.TopLeft);

            XFont headerFont = new XFont("Arial", 12, XFontStyle.Bold);
            XFont cellFont = new XFont("Arial", 10);

            XRect rect = new XRect(50, 100, page.Width - 100, page.Height - 200);
            int numberOfColumns = 3;
            int numberOfRows = model.ListRoomBookeds.Count;
            double columnWidth = rect.Width / numberOfColumns;
            double rowHeight = 20;

            // Vẽ tiêu đề
            gfx.DrawString("STT", headerFont, XBrushes.Black, new XRect(rect.Left, rect.Top, columnWidth, rowHeight), XStringFormats.CenterLeft);
            gfx.DrawString("Loại phòng", headerFont, XBrushes.Black, new XRect(rect.Left + columnWidth, rect.Top, columnWidth, rowHeight), XStringFormats.CenterLeft);
            gfx.DrawString("Giá", headerFont, XBrushes.Black, new XRect(rect.Left + columnWidth * 2, rect.Top, columnWidth, rowHeight), XStringFormats.CenterLeft);

            // Vẽ nội dung bảng
            for (int i = 0; i < numberOfRows; i++)
            {
                double top = rect.Top + rowHeight * (i + 1);

                gfx.DrawString((i + 1).ToString(), cellFont, XBrushes.Black, new XRect(rect.Left, top, columnWidth, rowHeight), XStringFormats.CenterLeft);
                gfx.DrawString(model.ListRoomBookeds[i].Name.ToString(), cellFont, XBrushes.Black, new XRect(rect.Left + columnWidth, top, columnWidth, rowHeight), XStringFormats.CenterLeft);
                gfx.DrawString(model.ListRoomBookeds[i].Price.ToString(), cellFont, XBrushes.Black, new XRect(rect.Left + columnWidth * 2, top, columnWidth, rowHeight), XStringFormats.CenterLeft);
            }
        }


        [HttpPost]
        public HttpResponseMessage ExportFilePdfOrder(ExportUserOrder model)
        {

            using (var memoryStream = new MemoryStream())
            {
                BindingFormatForPdfOrder(model);
                var nameFile = "";
                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(memoryStream.ToArray())
                };
                nameFile = "HoaDonMa" + model.OrderInfo.Code;
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = nameFile + ".pdf"
                };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                return result;
            }
        }
        
    }
}