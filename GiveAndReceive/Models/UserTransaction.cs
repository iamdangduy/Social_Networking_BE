namespace GiveAndReceive.Models
{
public class UserTransaction 
{
 public string UserTransactionId { get; set; }
 public string UserId { get; set; }
 public long Amount { get; set; }
 public string Note { get; set; }
 public long CreateTime { get; set; }
}
}
