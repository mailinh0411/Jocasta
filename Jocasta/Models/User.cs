using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jocasta.Models
{
    public class User
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool Enable { get; set; }
        public long CreateTime { get; set; }
    }
    public class ListUserView : BaseListModel
    {
        public List<User> List { get; set; }
    }
}