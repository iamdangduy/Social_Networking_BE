namespace GiveAndReceive.Models
{
    public class UserWareHouseDetail
    {
        public string UserWareHouseDetailId { get; set; }
        public string UserId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }
        public class EnumStatus
        {
            public const string NOT_RECEIVE = "NOT_RECEIVE";
            public const string RECEIVE = "RECEIVE";
        }
    }
}
