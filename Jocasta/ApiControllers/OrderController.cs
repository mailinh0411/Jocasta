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
    public class OrderController : ApiBaseController
    {
        // Danh sách số lượng phòng theo từng loại phòng còn trống khi đặt phòng 
        [HttpGet]
        public JsonResult GetListRoom(string checkIn, string checkOut, string keyword)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        string token = Request.Headers.Authorization.ToString();
                        UserService userService = new UserService(connect);
                        CartService cartService = new CartService(connect);
                        RoomCategoryService roomCategoryService = new RoomCategoryService(connect);
                        RoomService roomService = new RoomService(connect);
                        DayRoomService dayRoomService = new DayRoomService(connect);

                        User user = userService.GetUserByToken(token, transaction);
                        if (user == null) return Unauthorized();
                       
                        // Kiểm tra ngày checkin và checkout không được để trống.
                        if (string.IsNullOrEmpty(checkIn)) throw new Exception("Ngày check in không được để trống.");
                        if (string.IsNullOrEmpty(checkOut)) throw new Exception("Ngày check out không được để trống.");                        


                        DateTime CheckIn = Convert.ToDateTime(checkIn);
                        DateTime CheckOut = Convert.ToDateTime(checkOut);

                        long timeIn = HelperProvider.GetSeconds(CheckIn);
                        long timeOut = HelperProvider.GetSeconds(CheckOut);

                        if (CheckIn > CheckOut) throw new Exception("Ngày check in phải nhỏ hơn ngày check out.");

                        DateTime now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                        if (CheckIn < now || CheckOut < now) throw new Exception("Ngày check in, check out phải lớn hơn hoặc bằng ngày hiện tại.");

                        // Kiểm tra xem người dùng này đã có thêm phòng vào cart chưa 
                        Cart cart = cartService.GetCartByUserId(user.UserId, transaction);
                        if (cart == null)
                        {
                            cart = new Cart();
                            cart.CartId = Guid.NewGuid().ToString();
                            cart.UserId = user.UserId;
                            cart.TotalQuantity = 0;
                            cart.TotalPrice = 0;
                            cart.CheckIn = timeIn;
                            cart.CheckOut = timeOut;
                            cartService.InsertCart(cart, transaction);
                        }
                        else
                        {
                            // Kiểm tra xem cart đã có theo đúng ngày checkin và checkout
                            Cart cartCheck = cartService.GetCartByUserCheckInCheckOut(user.UserId, timeIn, timeOut, transaction);
                            if(cartCheck == null)
                            {
                                // Xóa toàn bộ cart detail và cart
                                if(cartService.GetListCartDetailByCart(cart.CartId, transaction).Count > 0)
                                {
                                    cartService.DeleteCartDetailByCart(cart.CartId, transaction);
                                }  
                                
                                cartService.DeleteCart(cart.CartId, transaction);

                                // Tạo ra cart mới
                                cart = new Cart();
                                cart.CartId = Guid.NewGuid().ToString();
                                cart.UserId = user.UserId;
                                cart.TotalQuantity = 0;
                                cart.TotalPrice = 0;
                                cart.CheckIn = timeIn;
                                cart.CheckOut = timeOut;
                                cartService.InsertCart(cart, transaction);
                            }
                        }

                        List<RoomInCart> listRoomInCart = new List<RoomInCart>();

                        List<RoomCategory> roomCategories = roomCategoryService.GetAllByKeyword(keyword, transaction);

                        foreach (RoomCategory index in roomCategories)
                        {
                            // Lấy ra danh sách phòng trống theo từng loại phòng
                            RoomInCart roomInCart = new RoomInCart();
                            roomInCart.RoomAvaiable = roomService.GetListRoomAvailable(index.RoomCategoryId, CheckIn, CheckOut, transaction);

                            // Lấy ra số lượng đã chọn theo từng loại phòng
                            CartDetail cartDetail = cartService.GetRoomBookedByCartRoom(cart.CartId, index.RoomCategoryId, transaction);
                            if(cartDetail == null)
                            {
                                roomInCart.CountSelect = 0;
                            }
                            else
                            {
                                roomInCart.CountSelect = cartDetail.Quantity;
                            }

                            listRoomInCart.Add(roomInCart);                            
                        }

                        transaction.Commit();
                        return Success(listRoomInCart);
                    }
                }
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }


        // Tao order dat phong
        [HttpPost]
        public JsonResult CreateOrder(CreateOrder model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        string token = Request.Headers.Authorization.ToString();
                        UserService userService = new UserService(connect);
                        CartService cartService = new CartService(connect);
                        OrderService orderService = new OrderService(connect);
                        InvoiceService invoiceService = new InvoiceService(connect);
                        RoomService roomService = new RoomService(connect);
                        DayRoomService dayRoomService = new DayRoomService(connect);    

                        User user = userService.GetUserByToken(token,transaction);
                        if (user == null) return Unauthorized();

                        Cart cart = cartService.GetCartByUserId(user.UserId, transaction);

                        if (cart == null) throw new Exception("Người dùng này chưa chọn phòng để đặt");

                        List<CartDetailModel> cartDetail = cartService.GetListRoomBookedByCart(cart.CartId, transaction);
                        if (cartDetail.Count == 0) throw new Exception("Người dùng này chưa chọn phòng để đặt");

                        // Tạo order 
                        Order order = new Order();
                        order.OrderId = Guid.NewGuid().ToString();
                        order.UserId = user.UserId;
                        order.TotalPrice = cart.TotalPrice;
                        order.Status = Order.EnumStatus.BOOKED;
                        order.CheckIn = cart.CheckIn;
                        order.CheckOut = cart.CheckOut;
                        order.Code = HelperProvider.MakeCode();
                        order.Email = model.Email;
                        order.Phone = model.Phone;
                        order.Name = model.Name;
                        order.CreateTime = HelperProvider.GetSeconds();
                        orderService.InsertOrder(order, transaction);

                        // Tạo invoice 
                        Invoice invoice = new Invoice();
                        invoice.InvoiceId = Guid.NewGuid().ToString();
                        invoice.OrderId = order.OrderId;
                        invoice.UserId = user.UserId;
                        invoice.TotalPrice = order.TotalPrice;
                        invoice.Type = Invoice.EnumType.BOOKING_INVOICE;
                        invoice.RequestContent = model.RequestContent;
                        invoice.CreateTime = HelperProvider.GetSeconds();
                        invoiceService.InsertInvoice(invoice, transaction);

                        DateTime checkIn = HelperProvider.GetDateTime(cart.CheckIn);
                        DateTime checkOut = HelperProvider.GetDateTime(cart.CheckOut);


                        // Tạo order_detail và invoice detail
                        foreach (var item in cartDetail)
                        {
                            OrderDetail orderDetail = new OrderDetail();
                            orderDetail.OrderDetailId = Guid.NewGuid().ToString();
                            orderDetail.OrderId = order.OrderId;
                            orderDetail.RoomCategoryId = item.RoomCategoryId;
                            orderDetail.NumberOfRoom = item.Quantity;
                            orderService.InsertOrderDetail(orderDetail, transaction);

                            InvoiceDetail invoiceDetail = new InvoiceDetail();
                            invoiceDetail.InvoiceDetailId = Guid.NewGuid().ToString();
                            invoiceDetail.InvoiceId = invoice.InvoiceId;
                            invoiceDetail.RoomCategoryId = item.RoomCategoryId;
                            invoiceDetail.Quantity = item.Quantity;
                            invoiceDetail.Price = item.Price;
                            invoiceService.InsertInvoiceDetail(invoiceDetail, transaction);

                            // Giữ chỗ cho khách hàng thêm vào bảng day_room
                            // Lấy ra danh sách phòng trống
                            CategoryRoomAvaiable categoryRoomAvaiable = roomService.GetListRoomAvailable(item.RoomCategoryId, checkIn, checkOut, transaction);

                            // Danh sách phòng trống
                            List<Room> rooms = categoryRoomAvaiable.ListRoom;

                            for(int i = 0; i< item.Quantity; i++)
                            {
                                // Random chọn phòng cho khách và thêm order và sửa status vào bảng day_room để giữ phòng cho khách 
                                Random rand = new Random();
                                int randRoom = rand.Next(0, rooms.Count);
                                
                                Room room = rooms[randRoom];

                                for (DateTime dateIndex = checkIn; dateIndex < checkOut;)
                                {
                                    long date = HelperProvider.GetSeconds(dateIndex);
                                    DayRoom dayRoom = dayRoomService.GetDayRoomByRoomAndDate(room.RoomId, date, transaction);
                                    if (dayRoom == null) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                                    dayRoom.OrderDetailId = orderDetail.OrderDetailId;
                                    dayRoom.Status = DayRoom.EnumStatus.BOOKED;
                                    dayRoomService.UpdateDayRoom(dayRoom, transaction);
                                    
                                    dateIndex = dateIndex.AddDays(1);
                                }

                                // Xóa phòng đó ra khỏi phòng trống.
                                rooms.RemoveAt(randRoom);
                            }
                        }

                        // Tạo xong đơn hàng thì xóa giỏ hàng
                        cartService.DeleteCartDetailByCart(cart.CartId, transaction);
                        cartService.DeleteCart(cart.CartId, transaction);

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

        // Danh sách order của người dùng
        [HttpGet]
        public JsonResult GetListOrderUser()
        {
            try
            {
                string token = Request.Headers.Authorization.ToString();
                UserService userService = new UserService();
                OrderService orderService = new OrderService(); 

                User user = userService.GetUserByToken(token);
                if (user == null) return Unauthorized();

                List<Order> orders = orderService.GetListOrderByUserId(user.UserId);
                return Success(orders);
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
                OrderService orderService = new OrderService();
                return Success(orderService.GetOrderById(id));
            }catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult UserCancelOrder(string orderId)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        OrderService orderService = new OrderService(connect);

                        Order order = orderService.GetOrderById(orderId, transaction);
                        if (order == null) throw new Exception("Không có đơn đặt của đơn này.");

                        if (order.Status != Order.EnumStatus.BOOKED) throw new Exception("Bạn không thể hủy đơn đặt này.");
                        order.Status = Order.EnumStatus.USER_CANCEL;

                        orderService.UpdateStatusOrder(order.OrderId, order.Status, transaction);

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