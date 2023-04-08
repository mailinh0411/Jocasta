using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jocasta.Models
{
    public class Service
    {
        public string ServiceId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public bool Enable { get; set; }
        public long CreateTime { get; set; }
    }

    public class ListServiceView : BaseListModel
    {
        public List<Service> List { get; set; }
    }
}