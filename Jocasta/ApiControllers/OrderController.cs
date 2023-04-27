using Jocasta.Models;
using Jocasta.Providers;
using Jocasta.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;


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

                        List<CategoryCountRoom> listCategoryCountRoom = new List<CategoryCountRoom>();

                        List<RoomCategory> roomCategories = roomCategoryService.GetAllByKeyword(keyword, transaction);

                        int count = 0;
                        bool checkBook = false;
                        foreach (RoomCategory index in roomCategories)
                        {
                            CategoryCountRoom categoryCountRoom = new CategoryCountRoom();
                            categoryCountRoom.Category = index;
                            List<Room> rooms = roomService.GetListRoomByCategory(index.RoomCategoryId, transaction);
                            // Gán đầu tiên số lượng phòng bằng 0
                            count = 0;
                            foreach (Room room in rooms)
                            {
                                if(room.Enable == false) continue;
                                // Nếu từ khoảng StartDate đến EndDate có phòng đã đặt thì gán checkBook bằng true, ngược lại thì count được cộng thêm 1 
                                for (DateTime dateIndex = CheckIn; dateIndex < CheckOut;)
                                {
                                    checkBook = false;
                                    long date = HelperProvider.GetSeconds(dateIndex);
                                    DayRoom dayRoom = dayRoomService.CheckDayRoomAvailable(room.RoomId, date, transaction);
                                    if(dayRoom != null && dayRoom.Status == DayRoom.EnumStatus.BOOKED)
                                    {
                                        checkBook = true;
                                        break;
                                    }
                                    dateIndex = dateIndex.AddDays(1);
                                }

                                if (checkBook == false) count += 1;
                            }

                            // Số lượng phòng trống theo từng loại phòng
                            categoryCountRoom.Count = count;

                            // Lấy ra số lượng đã chọn theo từng loại phòng
                            CartDetail cartDetail = cartService.GetRoomBookedByCartRoom(cart.CartId, index.RoomCategoryId, transaction);
                            if(cartDetail == null)
                            {
                                categoryCountRoom.CountSelect = 0;
                            }
                            else
                            {
                                categoryCountRoom.CountSelect = cartDetail.Quantity;
                            }

                            listCategoryCountRoom.Add(categoryCountRoom);                            
                        }

                        transaction.Commit();
                        return Success(listCategoryCountRoom);
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
        public JsonResult CreateOrder()
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

                        User user = userService.GetUserByToken(token,transaction);
                        if (user == null) return Unauthorized();

                        Cart cart = cartService.GetCartByUserId(user.UserId, transaction);

                        if (cart == null) throw new Exception("Người dùng này chưa chọn phòng để đặt");

                        List<CartDetailModel> cartDetail = cartService.GetListRoomBookedByCart(cart.CartId, transaction);
                        if (cartDetail.Count == 0) throw new Exception("Người dùng này chưa chọn phòng để đặt");


                        Order order = new Order();
                        order.OrderId = Guid.NewGuid().ToString();
                        order.UserId = user.UserId;
                        order.TotalPrice = cart.TotalPrice;
                        order.Status = Order.EnumStatus.PENDING;
                        order.CheckIn = cart.CheckIn;
                        order.CheckOut = cart.CheckOut;
                        order.CreateTime = HelperProvider.GetSeconds();
                        orderService.InsertOrder(order, transaction);

                        foreach (var item in cartDetail)
                        {
                            OrderDetail orderDetail = new OrderDetail();
                            orderDetail.OrderDetailId = Guid.NewGuid().ToString();
                            orderDetail.OrderId = order.OrderId;
                            orderDetail.RoomCategoryId = item.RoomCategoryId;
                            orderDetail.NumberOfRoom = item.Quantity;

                            orderService.InsertOrderDetail(orderDetail, transaction);
                        }

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