namespace GiveAndReceive.Models
{
    public class UserPinTransfer
    {
        public string UserPinTransferId { get; set; }
        public string UserGiveId { get; set; }
        public string UserReceiveId { get; set; }
        public int Pin { get; set; }
        public string Status { get; set; }
        public long CreateTime { get; set; }
        public string Message { get; set; }
        public class EnumStatus
        {
            public const string DONE = "DONE";
        }
    }

    public class TransferPin
    {
        public string UserId { get; set; }
        public int Pin { get; set; }
    }
}
