using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GiveAndReceive.Models
{
    public class CodeConfirm
    {
        public string CodeConfirmId { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public long ExpiryTime { get; set; }
        public long CreateTime { get; set; }
    }
}