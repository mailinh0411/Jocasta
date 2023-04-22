using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jocasta.Models
{
    public class UserToken
    {
        public string UserTokenId { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
        public long CreateTime { get; set; }
        public long ExpireTime { get; set; }
    }
}