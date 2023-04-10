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
}
}
