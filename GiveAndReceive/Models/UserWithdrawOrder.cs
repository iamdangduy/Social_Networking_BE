using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GiveAndReceive.Models
{
    public class UserWithdrawOrder
    {
        public string UserWithdrawOrderId { get; set; }
        public string UserId { get; set; }
        public string Code { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public long CreateTime { get; set; }
    }

    public class ListUserWithdrawOrderView
    {
        public List<UserWithdrawOrder> List { get; set; }
        public int TotalPage { get; set; }
    }
}