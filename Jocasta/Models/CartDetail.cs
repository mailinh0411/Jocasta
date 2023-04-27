using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jocasta.Models
{
    public class CartDetail
    {
        public string CartDetailId { get; set; }
        public string CartId { get; set; }
        public string RoomCategoryId { get; set; }
        public int Quantity { get; set; }
    }

    public class CartDetailModel
    {
        public string CartDetailId { get; set; }
        public string CartId { get; set; }
        public string RoomCategoryId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public class CreateCartDetail
    {
        public string RoomCategoryId { get; set; }
        public int Quantity { get; set; }
    }
}