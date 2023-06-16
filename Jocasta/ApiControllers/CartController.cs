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
    public class CartController : ApiBaseController
    {
        // GET: Cart
        [HttpGet]
        public JsonResult GetCartByUser()
        {
            try
            {
                string token = Request.Headers.Authorization.ToString();
                UserService userService = new UserService();
                CartService cartService = new CartService();

                User user = userService.GetUserByToken(token);
                if (user == null) return Unauthorized();

                Cart cart = cartService.GetCartByUserId(user.UserId);

                //if (cart == null) throw new Exception("Người dùng này chưa chọn phòng để đặt");

                return Success(cart);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        // Lấy ra danh sách phòng đã thêm vào cart
        [HttpGet]
        public JsonResult GetListRoomBookedByUser()
        {
            try
            {
                string token = Request.Headers.Authorization.ToString();
                UserService userService = new UserService();
                CartService cartService = new CartService();

                User user = userService.GetUserByToken(token);
                if (user == null) return Unauthorized();

                Cart cart = cartService.GetCartByUserId(user.UserId);
                List<CartDetailModel> cartDetail = new List<CartDetailModel>();
                if (cart != null)
                {
                    cartDetail = cartService.GetListRoomBookedByCart(cart.CartId);
                }
                
                return Success(cartDetail);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        // thêm phòng vào cart
        [HttpPost]
        public JsonResult InsertCartDetail(CreateCartDetail model)
        {
            try
            {
                using(var connect = BaseService.Connect())
                {
                    connect.Open();
                    using(var transaction  = connect.BeginTransaction())
                    {
                        string token = Request.Headers.Authorization.ToString();
                        UserService userService = new UserService(connect);
                        CartService cartService = new CartService(connect);
                        RoomCategoryService roomCategoryService = new RoomCategoryService(connect);

                        User user = userService.GetUserByToken(token, transaction);
                        if (user == null) return Unauthorized();

                        if (string.IsNullOrEmpty(model.RoomCategoryId)) throw new Exception("Bạn phải chọn phòng đặt.");
                        RoomCategory roomCategory = roomCategoryService.GetRoomCategoryById(model.RoomCategoryId, transaction);
                        if (roomCategory == null) throw new Exception("Phòng này không tồn tại.");

                        if (model.Quantity < 0) throw new Exception("Số lượng phòng đặt phải lớn hơn 0.");

                        Cart cart = cartService.GetCartByUserId(user.UserId, transaction);
                        if (cart == null) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        //Số ngày ở
                        TimeSpan difference = HelperProvider.GetDateTime(cart.CheckOut).Subtract(HelperProvider.GetDateTime(cart.CheckIn));
                        int totalDay = (int)difference.TotalDays;

                        // Kiểm tra xem đã cho loại phòng đó vào cart chưa
                        CartDetail cartDetail = cartService.GetRoomBookedByCartRoom(cart.CartId, model.RoomCategoryId, transaction);

                        // Kiểm tra xem có số lượng sửa lại có bằng 0
                        if (model.Quantity == 0)
                        {
                            // Nếu cartDetail tồn tại thì xóa cart detail của loại phòng đó và trừ đi tổng tiền và tổng số lượng phòng trong cart                            
                            if(cartDetail != null)
                            {
                                // Cập nhật lại tổng tiền và tổng số lượng trong cart
                                decimal price = cartDetail.Price * totalDay;
                                int quantity = cartDetail.Quantity;
                                cartService.UpdateCart(-price, -quantity, cart.CartId, transaction);
                                // Xóa cart detail của loại phòng đó
                                cartService.DeleteCartDetail(cartDetail.CartDetailId, transaction);                                
                            } 
                        }
                        else if(model.Quantity > 0)
                        {
                            // Kiểm tra xem cartDetail tồn tại không
                            // Nếu không thì thêm vào cart detail và cộng tổng tiền và tổng số lượng
                            if(cartDetail == null)
                            {
                                // thêm vào cart detail
                                CartDetail cartDetailInsert = new CartDetail();
                                cartDetailInsert.CartDetailId = Guid.NewGuid().ToString();
                                cartDetailInsert.CartId = cart.CartId;
                                cartDetailInsert.Quantity = model.Quantity;
                                cartDetailInsert.RoomCategoryId = model.RoomCategoryId;
                                cartDetailInsert.ExtraBed = 0;
                                cartDetailInsert.Price = model.Quantity * roomCategory.Price + cartDetailInsert.ExtraBed * Constant.EXTRA_BED * roomCategory.Price;
                                cartService.InsertCartDetail(cartDetailInsert, transaction);
                                // Cập nhật tổng tiền và số lượng của cart
                                // tổng tiền = số ngày ở * tiền
                                // số lượng = số lượng mới
                                decimal price = cartDetailInsert.Price * totalDay;
                                int quantity = cartDetailInsert.Quantity;
                                cartService.UpdateCart(price, quantity, cart.CartId, transaction);
                            }
                            else
                            {
                                // Cập nhật tổng tiền và số lượng của cart
                                // tổng tiền = số lượng mới * tiền * số ngày - số lượng cũ * tiền * số ngày
                                // số lượng = số lượng mới - số lượng cũ
                                decimal price = 0;
                                if (model.ExtraBed != cartDetail.ExtraBed)
                                {
                                    price = (model.Quantity * roomCategory.Price + model.ExtraBed * Constant.EXTRA_BED * roomCategory.Price) * totalDay - (cartDetail.Quantity * roomCategory.Price + cartDetail.ExtraBed * Constant.EXTRA_BED * roomCategory.Price) * totalDay;
                                }
                                else
                                {
                                    price = (model.Quantity * roomCategory.Price + cartDetail.ExtraBed * Constant.EXTRA_BED * roomCategory.Price) * totalDay - (cartDetail.Quantity * roomCategory.Price + cartDetail.ExtraBed * Constant.EXTRA_BED * roomCategory.Price) * totalDay;
                                }

                                int quantity = model.Quantity - cartDetail.Quantity;
                                cartService.UpdateCart(price, quantity, cart.CartId, transaction);
                                // Cập nhật lại số lượng phòng, tiền ở cart detail
                                cartDetail.Quantity = model.Quantity;
                                cartDetail.ExtraBed = model.ExtraBed;
                                cartDetail.Price = model.Quantity * roomCategory.Price + cartDetail.ExtraBed * Constant.EXTRA_BED * roomCategory.Price;
                                cartService.UpdateQuantityCartDetail(cartDetail, transaction);
                            }
                        }                        

                        transaction.Commit();
                        return Success();
                    }
                }
            }catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        // thêm phòng vào cart
        [HttpPost]
        public JsonResult UpdateCartDetail(UpdateCartDetail model)
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

                        User user = userService.GetUserByToken(token, transaction);
                        if (user == null) return Unauthorized();

                        if (string.IsNullOrEmpty(model.RoomCategoryId)) throw new Exception("Bạn phải chọn phòng đặt.");
                        RoomCategory roomCategory = roomCategoryService.GetRoomCategoryById(model.RoomCategoryId, transaction);
                        if (roomCategory == null) throw new Exception("Phòng này không tồn tại.");

                        if (model.ExtraBed < 0) throw new Exception("Số lượng phòng đặt phải lớn hơn 0.");

                        Cart cart = cartService.GetCartByUserId(user.UserId, transaction);
                        if (cart == null) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        //Số ngày ở
                        TimeSpan difference = HelperProvider.GetDateTime(cart.CheckOut).Subtract(HelperProvider.GetDateTime(cart.CheckIn));
                        int totalDay = (int)difference.TotalDays;

                        // Kiểm tra xem đã cho loại phòng đó vào cart chưa
                        CartDetail cartDetail = cartService.GetRoomBookedByCartRoom(cart.CartId, model.RoomCategoryId, transaction);
                        if(cartDetail == null) throw new Exception(JsonResult.Message.ERROR_SYSTEM);


                        // Cập nhật lại tổng tiền
                        decimal price = (cartDetail.Quantity * roomCategory.Price + model.ExtraBed * Constant.EXTRA_BED * roomCategory.Price) * totalDay - cartDetail.Price * totalDay;
                        int quantity = 0;
                        cartService.UpdateCart(price, quantity, cart.CartId, transaction);
                        // Cập nhật tổng tiền vào extrabed trong cartDetail
                        cartDetail.ExtraBed = model.ExtraBed;
                        cartDetail.Price = cartDetail.Quantity * roomCategory.Price + model.ExtraBed * Constant.EXTRA_BED * roomCategory.Price;
                        cartService.UpdateQuantityCartDetail(cartDetail, transaction);

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
