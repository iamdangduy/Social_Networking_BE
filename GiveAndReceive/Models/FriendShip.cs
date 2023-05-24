using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GiveAndReceive.Models
{
    public class FriendShip
    {
        public string FriendShipId { get; set; }
        public string UserId { get; set; }
        public string FriendId { get; set; }
        public string Status { get; set; }

        public class EnumStatus
        {
            public const string PENDING = "PENDING";
            public const string ACCEPTED = "ACCEPTED";
            public const string REJECTED = "REJECTED";
        }
    }
}