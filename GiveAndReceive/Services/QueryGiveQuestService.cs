using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using System.Data;
using GiveAndReceive.Models;
namespace GiveAndReceive.Services
{
    public class QueryGiveQuestService : BaseService
    {
        public QueryGiveQuestService() : base() { }
        public QueryGiveQuestService(IDbConnection db) : base(db) { }

        public List<QueueGiveQuest> GetListCurrentUserQuest(string queueGiveId, IDbTransaction transaction = null)
        {
            string query = "select * from queue_give_quest where QueueGiveId=@queueGiveId";
            return this._connection.Query<QueueGiveQuest>(query, new { queueGiveId }, transaction).ToList();
        }

        public void CancelQueueGiveQuest(string queueGiveId, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[queue_give_quest] SET [Status]=@status WHERE [QueueGiveId]=@queueGiveId and Status='" + QueueGiveQuest.EnumStatus.PENDING + "'";
            int status = this._connection.Execute(query, new { queueGiveId, status = QueueGiveQuest.EnumStatus.CANCEL }, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void InsertQueueGiveQuest(QueueGiveQuest model, IDbTransaction transaction = null) {
            string query = "INSERT INTO [dbo].[queue_give_quest] ([QueueGiveQuestId],[QueueGiveId],[QueueReceiveId],[AmountGive],[Status],[TransactionImage],[CreateTime],[ExpireTime]) VALUES (@QueueGiveQuestId,@QueueGiveId,@QueueReceiveId,@AmountGive,@Status,@TransactionImage,@CreateTime,@ExpireTime)";
            int status = this._connection.Execute(query, model, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
    }
}