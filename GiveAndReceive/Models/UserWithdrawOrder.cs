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
        public long Amount { get; set; }
        public string Status { get; set; }
        public long CreateTime { get; set; }
        public class EnumStatus
        {
            public const string PENDING = "PENDING";
            public const string PROCESSING = "PROCESSING";
            public const string DONE = "DONE";
            public const string SYSTEM_DECLINE = "SYSTEM_DECLINE";
            public const string USER_CANCEL = "USER_CANCEL";
        }
    }

    public class UserWithdrawOrderModel
    {
        public string UserWithdrawOrderId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Code { get; set; }
        public long Amount { get; set; }
        public string Status { get; set; }
        public long CreateTime { get; set; }
    }

    public class ListUserWithdrawOrderView
    {
        public List<UserWithdrawOrderModel> List { get; set; }
        public int TotalPage { get; set; }
    }
}