using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GiveAndReceive.Models
{
    public class UserAdmin
    {
        public string UserAdminId { get; set; }
        public string Name { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public long CreateTime { get; set; }
    }
}