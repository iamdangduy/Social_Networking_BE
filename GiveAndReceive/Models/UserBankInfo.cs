namespace GiveAndReceive.Models
{
public class UserBankInfo 
{
 public string UserBankInfoId { get; set; }
 public string UserId { get; set; }
 public string BankName { get; set; }
 public string BankOwnerName { get; set; }
 public string BankNumber { get; set; }
 public string QRImage { get; set; }
 public bool IsDefault { get; set; }
}
}
