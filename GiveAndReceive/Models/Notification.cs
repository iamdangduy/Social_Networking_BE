namespace GiveAndReceive.Models
{
    public class Notification
    {
        public string NotificationId { get; set; }
        public string UserId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public string Link { get; set; }
        public long CreateTime { get; set; }
        public string MessageShort { get; set; }
    }
}
