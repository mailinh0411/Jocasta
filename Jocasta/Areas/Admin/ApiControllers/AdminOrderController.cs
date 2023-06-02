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


namespace Jocasta.Areas.Admin.ApiControllers
{
    public class AdminOrderController : ApiBaseAdminController
    {
        // GET: Admin/AdminRoom
        [HttpGet]
        public JsonResult GetListOrder(string keyword, string status, int page, int pageSize)
        {
            try
            {
                if (string.IsNullOrEmpty(status)) status = "";
                AdminOrderService adminOrderService = new AdminOrderService();
                return Success(adminOrderService.GetListOrder(keyword, status, page, pageSize));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetOrderById(string id)
        {
            try
            {
                AdminOrderService adminOrderService = new AdminOrderService();
                return Success(adminOrderService.GetOrderById(id));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }


        [HttpPost]
        public JsonResult SystemCancelOrder(SystemCancelOrder model)
        {
            try
            {
                using(var connect = BaseService.Connect())
                {
                    connect.Open();
                    using(var transaction = connect.BeginTransaction())
                    {
                        UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                        if (userAdmin == null) return Unauthorized();

                        AdminOrderService adminOrderService = new AdminOrderService(connect);
                        ManageUserService manageUserService = new ManageUserService(connect);
                        AdminOrderTransactionService adminOrderTransactionService = new AdminOrderTransactionService(connect);
                        AdminNotificationService adminNotificationService = new AdminNotificationService(connect);
                        AdminDayRoomService adminDayRoomService = new AdminDayRoomService(connect);

                        Order order = adminOrderService.GetOrderById(model.OrderId, transaction);
                        if (order == null) throw new Exception("Hóa đơn này không tồn tại.");

                        User user = manageUserService.GetUserById(order.UserId, transaction);
                        if (user == null) throw new Exception("Khách hàng này không tồn tại");

                        if (order.Status != Order.EnumStatus.BOOKED) throw new Exception("Bạn không thể chuyển trạng thái cho đơn đặt này.");

                        order.Status = Order.EnumStatus.SYSTEM_CANCEL;
                        adminOrderService.UpdateStatusOrder(order.OrderId, order.Status, transaction);

                        //Xóa danh sách phòng ngày đã giữ chỗ cho khách
                        // 1. Lấy ra danh sách chi tiết đơn đặt của đơn đặt
                        List<OrderDetail> listOrderDetail = adminOrderService.GetOrderDetailsByOrder(order.OrderId, transaction);
                        // 2. Xóa cột orderDetailId trong phòng ngày
                        foreach (OrderDetail index in listOrderDetail)
                        {
                            adminDayRoomService.UpdateDayRoomByOrderDetail(index.OrderDetailId, DayRoom.EnumStatus.AVAILABLE, transaction);
                        }

                        DateTime now = DateTime.Now;

                        OrderTransaction orderTransaction = new OrderTransaction();
                        orderTransaction.OrderTransactionId = Guid.NewGuid().ToString();
                        orderTransaction.OrderId = model.OrderId;
                        orderTransaction.UserAdminId = userAdmin.UserAdminId;
                        orderTransaction.Status = order.Status;
                        orderTransaction.Content = model.Content;
                        orderTransaction.CreateTime = HelperProvider.GetSeconds(now);
                        adminOrderTransactionService.InsertOrderTransaction(orderTransaction, transaction);

                        // Thông báo cho người dùng
                        Notification notification = new Notification();
                        notification.NotificationId = Guid.NewGuid().ToString();
                        notification.Content = model.Content;
                        notification.Title = "Đơn đặt có mã [" + order.Code + "] đã bị hủy.";
                        notification.UserId = order.UserId;
                        notification.CreateTime = HelperProvider.GetSeconds(now);
                        notification.IsRead = false;
                        adminNotificationService.InsertNotification(notification, transaction);

                        // Gửi mail thông báo cho người dùng
                        if (!string.IsNullOrEmpty(user.Email))
                            SMSProvider.SendOTPViaEmail(user.Email, "", "[MAI LINH HOTEL] THÔNG BÁO HỦY ĐƠN ĐẶT", "Thông báo tới khách hàng " + user.Name + " đơn đặt của bạn có mã là: [" + order.Code + "] đã bị hệ thống hủy. Lý do: " + model.Content);


                        transaction.Commit();
                        return Success();
                    }
                }
            }catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult CheckInOrder(string orderId)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                        if (userAdmin == null) return Unauthorized();

                        AdminOrderService adminOrderService = new AdminOrderService(connect);
                        AdminNotificationService adminNotificationService = new AdminNotificationService(connect);
                        AdminOrderTransactionService adminOrderTransactionService = new AdminOrderTransactionService(connect);

                        Order order = adminOrderService.GetOrderById(orderId, transaction);
                        if (order == null) throw new Exception("Hóa đơn này không tồn tại.");

                        DateTime now = DateTime.Now;
                        if (HelperProvider.GetSeconds(now) < order.CheckIn) throw new Exception("Đơn đặt phòng chưa đến ngày check in.");
                        

                        if (order.Status != Order.EnumStatus.BOOKED) throw new Exception("Bạn không thể chuyển trạng thái cho đơn đặt này.");
                        order.Status = Order.EnumStatus.CHECKED_IN;

                        adminOrderService.UpdateStatusOrder(order.OrderId, order.Status, transaction);

                        OrderTransaction orderTransaction = new OrderTransaction();
                        orderTransaction.OrderTransactionId = Guid.NewGuid().ToString();
                        orderTransaction.OrderId = orderId;
                        orderTransaction.UserAdminId = userAdmin.UserAdminId;
                        orderTransaction.Status = order.Status;
                        orderTransaction.CreateTime = HelperProvider.GetSeconds(now);
                        adminOrderTransactionService.InsertOrderTransaction(orderTransaction, transaction);

                        // Tạo thông báo cho người dùng
                        Notification notification = new Notification();
                        notification.NotificationId = Guid.NewGuid().ToString();
                        notification.Title = "Đơn đặt đã được check in";
                        notification.Content = "Đơn đặt có mã " + order.Code + " của bạn đã được check in ngày " + now.ToString();
                        notification.UserId = order.UserId;
                        notification.CreateTime = HelperProvider.GetSeconds(now);
                        notification.IsRead = false;
                        adminNotificationService.InsertNotification(notification, transaction);

                        transaction.Commit();
                        return Success();
                    }
                }
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult CheckOutOrder(string orderId)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                        if (userAdmin == null) return Unauthorized();

                        AdminOrderService adminOrderService = new AdminOrderService(connect);
                        AdminSystemWalletService adminSystemWalletService = new AdminSystemWalletService(connect);
                        AdminSystemTransactionService adminSystemTransactionService = new AdminSystemTransactionService(connect);
                        AdminReportService adminReportService = new AdminReportService(connect);
                        AdminNotificationService adminNotificationService = new AdminNotificationService(connect);
                        AdminOrderTransactionService adminOrderTransactionService = new AdminOrderTransactionService(connect);

                        Order order = adminOrderService.GetOrderById(orderId, transaction);
                        if (order == null) throw new Exception("Hóa đơn này không tồn tại.");

                        if (order.Status != Order.EnumStatus.CHECKED_IN) throw new Exception("Bạn không thể chuyển trạng thái cho đơn đặt này.");
                        order.Status = Order.EnumStatus.CHECKED_OUT;
                        adminOrderService.UpdateStatusOrder(order.OrderId, order.Status, transaction);

                        DateTime now = DateTime.Now;
                        OrderTransaction orderTransaction = new OrderTransaction();
                        orderTransaction.OrderTransactionId = Guid.NewGuid().ToString();
                        orderTransaction.OrderId = orderId;
                        orderTransaction.UserAdminId = userAdmin.UserAdminId;
                        orderTransaction.Status = order.Status;
                        orderTransaction.CreateTime = HelperProvider.GetSeconds(now);
                        adminOrderTransactionService.InsertOrderTransaction(orderTransaction, transaction);

                        // Cộng tiền thu của hóa đơn vào trong ví khách sạn, và báo cáo
                        adminSystemWalletService.UpdateRevenueSystem(order.TotalPrice, transaction);

                        SystemTransaction systemTransaction = new SystemTransaction();
                        systemTransaction.SystemTransactionId = Guid.NewGuid().ToString();
                        systemTransaction.Amount = order.TotalPrice;
                        systemTransaction.Note = "Tiền thu từ hóa đơn của khách hàng " + order.Name + " đã check out.";
                        systemTransaction.CreateTime = HelperProvider.GetSeconds(now);
                        adminSystemTransactionService.InsertSystemTransaction(systemTransaction, transaction);

                        // Thêm vào bảng báo cáo
                        ReportDaily reportDaily = adminReportService.GetReportDailyByDayMonthYear(now.Day, now.Month, now.Year, transaction);
                        if(reportDaily == null)
                        {
                            reportDaily = new ReportDaily();
                            reportDaily.ReportDailyId = Guid.NewGuid().ToString();  
                            reportDaily.TotalPrice = order.TotalPrice;
                            reportDaily.Day = now.Day;
                            reportDaily.Month = now.Month;
                            reportDaily.Year = now.Year;
                            adminReportService.InsertReportDaily(reportDaily, transaction);
                        }
                        else
                        {
                            adminReportService.UpdateTotalPriceByReportDailyId(order.TotalPrice, reportDaily.ReportDailyId, transaction);
                        }
                        ReportMonthly reportMonthly = adminReportService.GetReportMonthlyByMonthYear(now.Month, now.Year, transaction);
                        if(reportMonthly == null)
                        {
                            reportMonthly = new ReportMonthly();
                            reportMonthly.ReportMonthlyId = Guid.NewGuid().ToString();
                            reportMonthly.TotalPrice = order.TotalPrice;
                            reportMonthly.Month = now.Month;
                            reportMonthly.Year = now.Year;
                            adminReportService.InsertReportMonthly(reportMonthly, transaction);
                        }
                        else
                        {
                            adminReportService.UpdateTotalPriceByReportMonthlyId(order.TotalPrice, reportMonthly.ReportMonthlyId, transaction);
                        }
                        ReportYearly reportYearly = adminReportService.GetReportYearlyByYear(now.Year, transaction);
                        if(reportYearly == null)
                        {
                            reportYearly = new ReportYearly();
                            reportYearly.ReportYearlyId = Guid.NewGuid().ToString();
                            reportYearly.TotalPrice = order.TotalPrice;
                            reportYearly.Year = now.Year;
                            adminReportService.InsertReportYearly(reportYearly, transaction);
                        }
                        else
                        {
                            adminReportService.UpdateTotalPriceByReportYearlyId(order.TotalPrice, reportYearly.ReportYearlyId, transaction);
                        }

                        // Tạo thông báo cho người dùng
                        Notification notification = new Notification();
                        notification.NotificationId = Guid.NewGuid().ToString();
                        notification.Title = "Đơn đặt đã được check out";
                        notification.Content = "Đơn đặt có mã " + order.Code + " của bạn đã được check out ngày " + now.ToString();
                        notification.UserId = order.UserId;
                        notification.CreateTime = HelperProvider.GetSeconds(now);
                        notification.IsRead = false;
                        adminNotificationService.InsertNotification(notification, transaction);


                        transaction.Commit();
                        return Success();
                    }
                }
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}