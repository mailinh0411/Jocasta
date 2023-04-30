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
            string query = "INSERT INTO [dbo].[invoice] ([InvoiceId],[UserId],[OrderId],[TotalPrice],[CreateTime],[Status],[Type],[RequestContent]) " +
                "VALUES (@InvoiceId,@UserId,@OrderId,@TotalPrice,@CreateTime,@Status,@Type,@RequestContent)";
            int status = this._connection.Execute(query, invoice, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
        #endregion

        #region InvoiceDetail
        public void InsertInvoiceDetail(InvoiceDetail invoiceDetail, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[invoice_detail] ([InvoiceDetailId],[InvoiceId],[ServiceId],[Price]) VALUES (@InvoiceDetailId,@InvoiceId,@ServiceId,@Price)";
            int status = this._connection.Execute(query, invoiceDetail, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
        #endregion
    }
}