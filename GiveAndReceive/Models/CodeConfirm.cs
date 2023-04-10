using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GiveAndReceive.Models
{
    public class CodeConfirm
    {
        public string CodeConfirmId { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public DateTime ExpiryTime { get; set; }
        public DateTime CreateTime { get; set; }
    }
}