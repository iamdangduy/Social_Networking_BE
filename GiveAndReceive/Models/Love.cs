using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GiveAndReceive.Models
{
    public class Love
    {
        public string LoveId { get; set; }
        public string PostId { get; set; }
        public string UserId { get; set; }
        public long CreateTime { get; set; }
    }
}