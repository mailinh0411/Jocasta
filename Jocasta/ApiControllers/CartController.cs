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

                if (cart == null) throw new Exception("Người dùng này chưa chọn phòng để đặt");

                return Success(cart);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

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

                        //if(model.Quantity)


                        Cart cart = cartService.GetCartByUserId(user.UserId, transaction);

                        if(cart == null)
                        {
                            cart = new Cart();
                            cart.CartId = Guid.NewGuid().ToString();
                            cart.UserId = user.UserId;
                            cart.TotalQuantity = model.Quantity;
                            cart.TotalPrice = model.Quantity * roomCategory.Price;
                            DateTime checkIn = Convert.ToDateTime(model.CheckIn);
                            DateTime checkOut = Convert.ToDateTime(model.CheckOut);
                            cart.CheckIn = HelperProvider.GetSeconds(checkIn);
                            cart.CheckOut = HelperProvider.GetSeconds(checkOut);
                            cartService.InsertCart(cart, transaction);
                        }
                        else
                        {
                            cart.TotalPrice = model.Quantity * roomCategory.Price;
                            cart.TotalQuantity = model.Quantity;
                            cartService.UpdateCart(cart, transaction);
                        }

                        CartDetail cartDetail = cartService.GetRoomBookedByCartRoom(cart.CartId, model.RoomCategoryId, transaction);
                        if(cartDetail == null)
                        {
                            cartDetail = new CartDetail();
                            cartDetail.CartDetailId = Guid.NewGuid().ToString();
                            cartDetail.CartId = cart.CartId;
                            cartDetail.Quantity = model.Quantity;
                            cartDetail.RoomCategoryId = model.RoomCategoryId;
                            cartService.InsertCartDetail(cartDetail, transaction);
                        }
                        else
                        {
                            cartDetail.Quantity = model.Quantity;
                            if(cartDetail.Quantity == 0)
                            {
                                cartService.DeleteCartDetail(cartDetail.CartDetailId, transaction);
                            }
                            else
                            {
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
    }
}