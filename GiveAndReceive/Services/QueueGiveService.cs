using Dapper;
using GiveAndReceive.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc.Ajax;

namespace GiveAndReceive.Services
{
    public class QueueGiveService : BaseService
    {
        public QueueGiveService() : base() { }
        public QueueGiveService(IDbConnection db) : base(db) { }
        public QueueGive GetQueueGiveByUserId(string userId, IDbTransaction transaction = null)
        {
            string query = "select TOP(1)* from [queue_give] where UserId=@userId order by CreateTime desc";
            return this._connection.Query<QueueGive>(query,new { userId }, transaction).FirstOrDefault();
        }
        public QueueGive GetQueueGive(string queueGiveId, IDbTransaction transaction = null)
        {
            string query = "select TOP(1) * from [queue_give] where QueueGiveId=@queueGiveId";
            return this._connection.Query<QueueGive>(query, new { queueGiveId }, transaction).FirstOrDefault();
        }

        public void InsertQueueGive(QueueGive queueGive, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[queue_give] ([QueueGiveId],[UserId],[Status],[CreateTime]) VALUES (@QueueGiveId,@UserId,@Status,@CreateTime)";
            int status = this._connection.Execute(query, queueGive, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void UpdateQueueGive(QueueGive queueGive, IDbTransaction transaction = null) {
            string query = "UPDATE [dbo].[queue_give] SET [Status]=@Status WHERE [QueueGiveId]=@QueueGiveId";
            int status = this._connection.Execute(query, queueGive, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public QueueGive GetQueueGiveInduty(IDbTransaction transaction = null)
        {
            string query = "select top 1 * from queue_give where Status=@status";
            return this._connection.Query<QueueGive>(query, new { status = QueueGive.EnumStatus.IN_DUTY }, transaction).FirstOrDefault();
        }

        public QueueGive GetFirstPendingInQueue(IDbTransaction transaction = null) {
            string query = "select top 1 * from queue_give where Status=@status order by CreateTime asc";
            return this._connection.Query<QueueGive>(query, new { status = QueueGive.EnumStatus.PENDING }, transaction).FirstOrDefault();
        }

        public QueueGive GetUserQueueGiveInDuty(string userId, IDbTransaction transaction = null) {
            string query = "select top 1 * from queue_give where Status=@status and UserId=@userId";
            return this._connection.Query<QueueGive>(query, new { userId, status =QueueGive.EnumStatus.IN_DUTY}, transaction).FirstOrDefault();
        }
    }
}