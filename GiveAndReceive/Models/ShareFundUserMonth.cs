using System.Collections.Generic;

namespace GiveAndReceive.Models
{
    public class ShareFundUserMonth
    {
        public string ShareFundUserMonthId { get; set; }
        public string ShareFundMonthId { get; set; }
        public string UserId { get; set; }
        public long Amount { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string Status { get; set; }
    }

    public class ListShareFundUserMonthView
    {
        public List<ShareFundUserMonth> List { get; set; }
        public int TotalPage { get; set; }
    }
}
