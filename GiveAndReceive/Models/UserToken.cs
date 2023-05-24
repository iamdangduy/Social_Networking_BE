namespace GiveAndReceive.Models
{
    public class UserToken
    {
        public string UserTokenId { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
        public long ExpireTime { get; set; }
        public long CreateTime { get; set; }
    }
}
