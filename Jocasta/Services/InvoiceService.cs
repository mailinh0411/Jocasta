using Dapper;
using Jocasta.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Jocasta.Services
{
    public class InvoiceService : BaseService
    {
        public InvoiceService() : base() { }
        public InvoiceService(IDbConnection db) : base(db) { }

        #region Invoice
        public void InsertInvoice(Invoice invoice, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[invoice] ([InvoiceId],[UserId],[OrderId],[TotalPrice],[CreateTime],[Type],[RequestContent]) " +
                "VALUES (@InvoiceId,@UserId,@OrderId,@TotalPrice,@CreateTime,@Type,@RequestContent)";
            int status = this._connection.Execute(query, invoice, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public Invoice GetInvoiceBooking(string orderId, IDbTransaction transaction = null)
        {
            string query = $"select * from [dbo].[invoice] where OrderId = @orderId and Type = '{Invoice.EnumType.BOOKING_INVOICE}'";
            return this._connection.Query<Invoice>(query, new { orderId }, transaction).FirstOrDefault();
        }
        #endregion

        #region InvoiceDetail
        public void InsertInvoiceDetail(InvoiceDetail invoiceDetail, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[invoice_detail] ([InvoiceDetailId],[InvoiceId],[RoomCategoryId],[ServiceId],[Quantity],[Price]) VALUES (@InvoiceDetailId,@InvoiceId,@RoomCategoryId,@ServiceId,@Quantity,@Price)";
            int status = this._connection.Execute(query, invoiceDetail, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public List<InvoiceDetail> GetInvoiceDetailByInvoiceId(string invoiceId, IDbTransaction transaction = null)
        {
            string query = "select * from [invoice_detail] where InvoiceId = @invoiceId";
            return this._connection.Query<InvoiceDetail>(query, new {invoiceId }, transaction).ToList();
        }

        public object GetListBookingService(string invoiceId, IDbTransaction transaction = null)
        {
            string query = "select ind.*, rc.Image, rc.Name from [invoice_detail] ind left join [room_category] rc on ind.RoomCategoryId = rc.RoomCategoryId where InvoiceId = @invoiceId";
            return this._connection.Query<object>(query, new { invoiceId }, transaction).ToList();
        }
        #endregion
    }
}