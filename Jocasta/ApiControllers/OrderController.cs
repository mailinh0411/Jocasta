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
                        RoomCategoryService roomCategoryService = new RoomCategoryService(connect);
                        RoomService roomService = new RoomService(connect);
                        DayRoomService dayRoomService = new DayRoomService(connect);

                        List<CategoryCountRoom> listCategoryCountRoom = new List<CategoryCountRoom>();

                        List<RoomCategory> roomCategories = roomCategoryService.GetAllByKeyword(keyword, transaction);

                        DateTime CheckIn = Convert.ToDateTime(checkIn);
                        DateTime CheckOut = Convert.ToDateTime(checkOut);
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
                                for (DateTime dateIndex = CheckIn; dateIndex <= CheckOut;)
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

                            categoryCountRoom.Count = count;
                            listCategoryCountRoom.Add(categoryCountRoom);                            
                        }

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
                        order.CreateTime = HelperProvider.GetSeconds();
                        orderService.InsertOrder(order, transaction);

                        foreach (var item in cartDetail)
                        {
                            OrderDetail orderDetail = new OrderDetail();
                            orderDetail.OrderDetailId = Guid.NewGuid().ToString();
                            orderDetail.OrderId = order.OrderId;
                            orderDetail.RoomCategoryId = item.RoomCategoryId;
                            orderDetail.NumberOfRoom = item.Quantity;
                            orderDetail.CheckIn = item.CheckIn;
                            orderDetail.CheckOut = item.CheckOut;

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