namespace GiveAndReceive.Models
{
    public class User
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string CoverPhoto { get; set; }
        public string Account { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public long DateOfBirth { get; set; }
        public string Gender { get; set; }
        public long CreateTime { get; set; }
        public string Address { get; set; }
    }
    public class UserLoginPost
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class UserRequest
    {
        public string Password { get; set; }
        public string NewPassword { get; set; }
    }

    public class UpdateUser
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string CoverPhoto { get; set; }
        public string Account { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string EmailCode { get; set; }
        public string Phone2 { get; set; }
        public string Address { get; set; }
    }

}
