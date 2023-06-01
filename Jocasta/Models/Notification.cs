using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jocasta.Models
{
    public class Notification
    {
        public string NotificationId { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
        public string UserId { get; set; }
        public string Link { get; set; }
        public long CreateTime { get; set; }
        public bool IsRead { get; set; }
    }
}