﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jocasta.Models
{
    public class RoomCategory
    {
        public string RoomCategoryId { get; set; }
        public string Name { get; set; }
        public string View { get; set; }
        public decimal Square { get; set; }
        public int NumberOfPeople { get; set; }
        public int SingleBed { get; set; }
        public int DoubleBed { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public bool Enable { get; set; }
        public long CreateTime { get; set; }
        public string Image { get; set; }
        public string SearchName { get; set; }
    }

    public class RoomCategoryModel : RoomCategory
    {
        public string[] ListImage { get; set; }
        public string[] ListImageDelete { get; set; }
    }

    public class ListRoomCategoryView : BaseListModel
    {
        public List<RoomCategory> List { get; set; }
    }

    public class RoomCategorySearchModel : SearchModel
    {
        public string Keyword { get; set; }
    }

    public class CategoryRoomAvaiable
    {
        public RoomCategory Category { get; set; }
        public List<Room> ListRoom { get; set; }
       
    }

    public class RoomInCart
    {
        public CategoryRoomAvaiable RoomAvaiable { get; set; }
        public int CountSelect { get; set; }
        public int CountExtraBed { get; set; }
    }


    /*public class RoomCategoryCheckNow
    {
        public string RoomCategory { get; set; }
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
    }*/
}