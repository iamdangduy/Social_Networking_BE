namespace GiveAndReceive.Models
{
public class QueueReceive 
{
 public string QueueReceiveId { get; set; }
 public string UserId { get; set; }
 public string Status { get; set; }
 public long TotalReceivedAmount { get; set; }
 public long TotalExpectReceiveAmount { get; set; }
 public long CreateTime { get; set; }
}
}
