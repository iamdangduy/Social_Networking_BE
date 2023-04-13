namespace GiveAndReceive.Models
{
    public class QueueGive
    {
        public string QueueGiveId { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
        public long CreateTime { get; set; }

        public class EnumStatus
        {
            public const string PENDING = "PENDING";
            public const string IN_DUTY = "IN-DUTY";
            public const string DONE = "DONE";
            public const string CANCEL = "CANCEL";
        }
    }
}
