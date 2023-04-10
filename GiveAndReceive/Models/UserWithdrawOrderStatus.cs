using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GiveAndReceive.Models
{
    public class UserWithdrawOrderStatus
    {
        public string UserWithdrawOrderStatusId { get; set; }
        public string UserWithdrawOrderId { get; set; }
        public string Status { get; set; }
        public long CreateTime { get; set; }
    }
}