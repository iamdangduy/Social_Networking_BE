using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GiveAndReceive.Models
{
    public class SystemBankInfo
    {
        public string SystemBankInfoId { get; set; }
        public string BankName { get; set; }
        public string BankOwnerName { get; set; }
        public string BankNumber { get; set; }
        public bool IsDefault { get; set; }
    }
}