using Dapper;
using Jocasta.Models;
using Jocasta.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Jocasta.Areas.Admin.Services
{
    public class AdminInvoiceService : BaseService
    {
        public AdminInvoiceService() : base() { }   
        public AdminInvoiceService(IDbConnection db) : base(db) { }
        public Invoice GetInvoiceBooking(string orderId, IDbTransaction transaction = null)
        {
            string query = $"select * from [dbo].[invoice] where OrderId = @orderId and Type = '{Invoice.EnumType.BOOKING_INVOICE}'";
            return this._connection.Query<Invoice>(query, new { orderId }, transaction).FirstOrDefault();
        }
        public List<Invoice> GetInvoiceService(string orderId, IDbTransaction transaction = null)
        {
            string query = $"select * from [invoice] where OrderId = @orderId and Type = '{Invoice.EnumType.SERVICE_INVOICE}'";
            return this._connection.Query<Invoice>(query, new { orderId }, transaction).ToList();
        }
        public List<object> GetListServiceBooked(string invoiceId, IDbTransaction transaction = null)
        {
            string query = "select ind.*, inv.CreateTime, s.Name as ServiceName, r.Name as RoomName from [invoice] inv left join [invoice_detail] ind on inv.InvoiceId = ind.InvoiceId left join [service] s on ind.ServiceId = s.ServiceId left join [room] r on ind.RoomId = r.RoomId where inv.InvoiceId = @invoiceId order by inv.CreateTime desc";
            return this._connection.Query<object>(query, new { invoiceId }, transaction).ToList();
        }

        public object GetListBookingService(string invoiceId, IDbTransaction transaction = null)
        {
            string query = "select ind.*, rc.Image, rc.Name from [invoice_detail] ind left join [room_category] rc on ind.RoomCategoryId = rc.RoomCategoryId where InvoiceId = @invoiceId";
            return this._connection.Query<object>(query, new { invoiceId }, transaction).ToList();
        }
    }
}