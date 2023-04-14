using System.Collections.Generic;

namespace GiveAndReceive.Models
{
    public class ShareFundMonth
    {
        public string ShareFundMonthId { get; set; }
        public long Balance { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
    public class ListShareFundMonthView
    {
        public List<ShareFundMonth> List { get; set; }
        public int TotalPage { get; set; }
    }
}
