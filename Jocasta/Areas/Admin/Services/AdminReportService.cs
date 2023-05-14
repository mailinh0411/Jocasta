using Dapper;
using Jocasta.Models;
using Jocasta.Providers;
using Jocasta.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;

namespace Jocasta.Areas.Admin.Services
{
    public class AdminReportService : BaseService
    {
        public AdminReportService() : base() { }
        public AdminReportService(IDbConnection db) : base(db) { }

        #region ReportDaily
        public ReportDaily GetReportDailyByDayMonthYear(int day, int month, int year, IDbTransaction transaction = null)
        {
            string query = "select * from [dbo].[report_daily] where Day=@day and Month=@month and Year=@year";
            return this._connection.Query<ReportDaily>(query, new {day, month, year}, transaction).FirstOrDefault();
        }
        public void InsertReportDaily(ReportDaily model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[report_daily] ([ReportDailyId],[TotalPrice],[Day],[Month],[Year]) VALUES (@ReportDailyId,@TotalPrice,@Day,@Month,@Year)";
            int count = this._connection.Execute(query, model, transaction);
            if (count <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void UpdateTotalPriceByReportDailyId(decimal price, string id, IDbTransaction transaction = null)
        {
            string query = "update [dbo].[report_daily] set [TotalPrice] = [TotalPrice] + @price where [ReportDailyId] = @id";
            int count = this._connection.Execute(query, new {price, id}, transaction);
            if (count <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public object GetAllReportDaily(long fromDate, long toDate, IDbTransaction transaction = null)
        {
            string querySelect = "select * ";
            string querySum = "select SUM(TotalPrice) ";
            string query = "from [report_daily] where 1=1";
            if(fromDate > 0 && toDate > 0)
            {
                DateTime from = HelperProvider.GetDateTime(fromDate);
                DateTime to = HelperProvider.GetDateTime(toDate);

                query += " and DATEFROMPARTS(Year, Month, Day) >= DATEFROMPARTS(" + from.Year + ", " + from.Month + ", " + from.Day + ")";
                query += " and DATEFROMPARTS(Year, Month, Day) <= DATEFROMPARTS(" + to.Year + ", " + to.Month + ", " + to.Day + ")";
            }

            decimal? TotalAmount = this._connection.Query<decimal?>(querySum + query, new { fromDate, toDate }, transaction).FirstOrDefault();
            List<ReportDaily> ListAllReportDaily = this._connection.Query<ReportDaily>(querySelect + query, new { fromDate, toDate }, transaction).ToList();
            return new
            {
                ListAllReportDaily,
                TotalAmount
            };
        }
        #endregion

        #region ReportMonthly
        public ReportMonthly GetReportMonthlyByMonthYear(int month, int year, IDbTransaction transaction = null)
        {
            string query = "select * from [dbo].[report_monthly] where Month=@month and Year=@year";
            return this._connection.Query<ReportMonthly>(query, new {month, year}, transaction).FirstOrDefault();   
        }
        public void InsertReportMonthly(ReportMonthly model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[report_monthly] ([ReportMonthlyId],[TotalPrice],[Month],[Year]) VALUES (@ReportMonthlyId,@TotalPrice,@Month,@Year)";
            int count = this._connection.Execute(query, model, transaction);
            if (count <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
        public void UpdateTotalPriceByReportMonthlyId(decimal price, string id, IDbTransaction transaction = null)
        {
            string query = "update [dbo].[report_monthly] set [TotalPrice] = [TotalPrice] + @price where [ReportMonthlyId] = @id";
            int count = this._connection.Execute(query, new { price, id }, transaction);
            if (count <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
        public object GetAllReportMonth(long fromDate, long toDate, IDbTransaction transaction = null)
        {
            string querySelect = "select * ";
            string querySum = "select SUM(TotalPrice) ";
            string query = "from [report_daily] where 1=1";
            if (fromDate > 0 && toDate > 0)
            {
                DateTime from = HelperProvider.GetDateTime(fromDate);
                DateTime to = HelperProvider.GetDateTime(toDate);

                query += " and DATEFROMPARTS(Year, Month, 1) >= DATEFROMPARTS(" + from.Year + ", " + from.Month + ", 1)";
                query += " and DATEFROMPARTS(Year, Month, Day) <= DATEFROMPARTS(" + to.Year + ", " + to.Month + ", " + to.Day + ")";
            }

            decimal? TotalAmount = this._connection.Query<decimal?>(querySum + query, new { fromDate, toDate }, transaction).FirstOrDefault();
            List<ReportMonthly> ListAllReportMonth = this._connection.Query<ReportMonthly>(querySelect + query, new { fromDate, toDate }, transaction).ToList();
            return new
            {
                ListAllReportMonth,
                TotalAmount
            };
        }
        #endregion

        #region ReportYearly
        public ReportYearly GetReportYearlyByYear(int year, IDbTransaction transaction = null)
        {
            string query = "select * from [dbo].[report_yearly] where Year = @year";
            return this._connection.Query<ReportYearly>(query, new {year }, transaction).FirstOrDefault();
        }
        public void InsertReportYearly(ReportYearly model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[report_yearly] ([ReportYearlyId],[TotalPrice],[Year]) VALUES (@ReportYearlyId,@TotalPrice,@Year)";
            int count = this._connection.Execute(query, model, transaction);
            if (count <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
        public void UpdateTotalPriceByReportYearlyId(decimal price, string id, IDbTransaction transaction = null)
        {
            string query = "update [dbo].[report_yearly] set [TotalPrice] = [TotalPrice] + @price where [ReportYearlyId] = @id";
            int count = this._connection.Execute(query, new { price, id }, transaction);
            if (count <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
        #endregion
    }
}