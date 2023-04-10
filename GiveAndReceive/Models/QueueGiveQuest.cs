namespace GiveAndReceive.Models
{
public class QueueGiveQuest 
{
 public string QueueGiveQuestId { get; set; }
 public string QueueGiveId { get; set; }
 public string QueueReceiveId { get; set; }
 public long AmountGive { get; set; }
 public string Status { get; set; }
 public long CreateTime { get; set; }
 public long ExpireTime { get; set; }
}
}
