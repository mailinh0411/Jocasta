using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Jocasta.Services
{
    public class OrderService : BaseService
    {
        public OrderService() : base() { }
        public OrderService(IDbConnection db) : base(db) { }


    }
}