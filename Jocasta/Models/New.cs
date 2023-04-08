using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jocasta.Models
{
    public class New
    {
        public string NewId { get; set; }
        public string Image { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
        public string UserAdminId { get; set; }
        public long CreateTime { get; set; }
    }

    public class ListNewView : BaseListModel
    {
        public List<New> List { get; set; }
    }
}