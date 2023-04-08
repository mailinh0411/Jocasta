using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jocasta.Models
{
    public class UserAdmin
    {
        public string UserAdminId { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Account { get; set; }
        public string Phone {  get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public bool Enable { get; set; }
        public long CreateTime { get; set; }
    }
}