namespace GiveAndReceive.Models
{
    public class User
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Account { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public long CreateTime { get; set; }
        public string ShareCode { get; set; }
        public string ParentCode { get; set; }
    }

    public class UserRequest
    {
        public string Password { get; set; }
        public string NewPassword { get; set; }
    }
}
