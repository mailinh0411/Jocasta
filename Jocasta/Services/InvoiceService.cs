﻿using Dapper;
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

        public List<Invoice> GetInvoiceService(string orderId, IDbTransaction transaction = null)
        {
            string query = $"select * from [invoice] where OrderId = @orderId and Type = '{Invoice.EnumType.SERVICE_INVOICE}'";
            return this._connection.Query<Invoice>(query , new { orderId }, transaction).ToList();
        }
        #endregion

        #region InvoiceDetail
        public void InsertInvoiceDetail(InvoiceDetail invoiceDetail, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[invoice_detail] ([InvoiceDetailId],[InvoiceId],[RoomCategoryId],[ServiceId],[Quantity],[Price],[RoomId],[ExtraBed]) VALUES (@InvoiceDetailId,@InvoiceId,@RoomCategoryId,@ServiceId,@Quantity,@Price,@RoomId,@ExtraBed)";
            int status = this._connection.Execute(query, invoiceDetail, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public List<InvoiceDetail> GetInvoiceDetailByInvoiceId(string invoiceId, IDbTransaction transaction = null)
        {
            string query = "select ind.*, inv.CreateTime from [invoice_detail] ind join [invoice] inv on ind.InvoiceId = inv.InvoiceId where inv.InvoiceId = @invoiceId order by inv.CreateTime desc";
            return this._connection.Query<InvoiceDetail>(query, new {invoiceId }, transaction).ToList();
        }

        public List<object> GetListServiceBooked(string invoiceId, IDbTransaction transaction = null)
        {
            string query = "select ind.*, inv.CreateTime, s.Name as ServiceName, r.Name as RoomName from [invoice] inv left join [invoice_detail] ind on inv.InvoiceId = ind.InvoiceId left join [service] s on ind.ServiceId = s.ServiceId left join [room] r on ind.RoomId = r.RoomId where inv.InvoiceId = @invoiceId order by inv.CreateTime desc";
            return this._connection.Query<object>(query, new {invoiceId}, transaction).ToList();
        }

        public object GetListBookingService(string invoiceId, IDbTransaction transaction = null)
        {
            string query = "select ind.*, rc.Image, rc.Name from [invoice_detail] ind left join [room_category] rc on ind.RoomCategoryId = rc.RoomCategoryId where InvoiceId = @invoiceId";
            return this._connection.Query<object>(query, new { invoiceId }, transaction).ToList();
        }
        #endregion
    }
}