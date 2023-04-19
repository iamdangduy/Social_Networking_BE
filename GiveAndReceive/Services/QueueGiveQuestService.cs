using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using System.Data;
using GiveAndReceive.Models;
namespace GiveAndReceive.Services
{
    public class QueueGiveQuestService : BaseService
    {
        public QueueGiveQuestService() : base() { }
        public QueueGiveQuestService(IDbConnection db) : base(db) { }

        public List<QueueGiveQuest> GetListCurrentUserQuest(string queueGiveId, IDbTransaction transaction = null)
        {
            string query = "select * from queue_give_quest where QueueGiveId=@queueGiveId";
            return this._connection.Query<QueueGiveQuest>(query, new { queueGiveId }, transaction).ToList();
        }

        public QueueGiveQuest GetQueueGiveQuest(string queueGiveQuestId, IDbTransaction transaction = null)
        {
            string query = "SELECT * FROM [dbo].[queue_give_quest] where  [QueueGiveQuestId] = @queueGiveQuestId";
            return this._connection.Query<QueueGiveQuest>(query, new { queueGiveQuestId }, transaction).FirstOrDefault();
        }

        public List<object> GetListCurrentUserQuestView(string queueGiveId, IDbTransaction transaction = null)
        {
            string query = "select qgq.*,u.UserId,u.Avatar,u.Name,u.Email,u.Phone from [queue_give_quest] qgq left join [queue_receive] qr on qgq.QueueReceiveId=qr.QueueReceiveId left join [user] u on qr.UserId=u.UserId where qgq.QueueGiveId=@queueGiveId";
            return this._connection.Query<object>(query, new { queueGiveId }, transaction).ToList();
        }

        public void CancelQueueGiveQuest(string queueGiveId, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[queue_give_quest] SET [Status]=@status WHERE [QueueGiveId]=@queueGiveId and Status='" + QueueGiveQuest.EnumStatus.PENDING + "'";
            int status = this._connection.Execute(query, new { queueGiveId, status = QueueGiveQuest.EnumStatus.CANCEL }, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void InsertQueueGiveQuest(QueueGiveQuest model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[queue_give_quest] ([QueueGiveQuestId],[QueueGiveId],[QueueReceiveId],[AmountGive],[Status],[TransactionImage],[CreateTime],[ExpireTime],[Code]) VALUES (@QueueGiveQuestId,@QueueGiveId,@QueueReceiveId,@AmountGive,@Status,@TransactionImage,@CreateTime,@ExpireTime,@Code)";
            int status = this._connection.Execute(query, model, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void UpdateQueueGiveQuest(QueueGiveQuest model, IDbTransaction transaction = null)
        {
            string query = "update [dbo].[queue_give_quest] set Status=@status,TransactionImage=@TransactionImage where QueueGiveQuestId=@queueGiveQuestId";
            int _status = this._connection.Execute(query, model, transaction);
            if (_status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public object GetListUserGiveQuest(string userId, string status, int page, IDbTransaction transaction = null)
        {
            string querySelect = "select qgq.*,ug.Name,ug.Avatar ";
            string queryCount = "select count(*) as Total ";

            string query = " from [queue_give_quest] qgq left join [queue_receive] qr on qgq.QueueReceiveId = qr.QueueReceiveId";
            query += " left join [queue_give] qg on qgq.QueueGiveId = qg.QueueGiveId";
            query += " left join [user] ug on qg.UserId = ug.UserId";
            query += " where qr.UserId = @userId";

            if (!string.IsNullOrEmpty(status))
            {
                query += "  and qgq.Status = @status";
            }

            long totalRow = this._connection.Query<long>(queryCount + query, new { userId, status }).FirstOrDefault();
            long totalPage = (long)Math.Ceiling((decimal)totalRow / Constant.NUMBER.USER_PAGE_SIZE);


            int skip = (page - 1) * Constant.NUMBER.USER_PAGE_SIZE;
            query += " order by qgq.CreateTime desc offset " + skip + " rows fetch next " + Constant.NUMBER.USER_PAGE_SIZE + " rows only";
            List<object> listData = this._connection.Query<object>(querySelect + query, new { userId, status }, transaction).ToList();
            return new { totalPage, listData };
        }


        public object GetListUserQueueGive(string userId, IDbTransaction transaction = null)
        {
            string query = "select qgq.*, ug.UserId, ug.Name";
            query += " from [queue_give_quest] qgq left join [queue_give] qg on qgq.QueueGiveId = qg.QueueGiveId";
            query += " left join [queue_receive] qr on qgq.QueueReceiveId = qr.QueueReceiveId";
            query += " left join [user] ug on qr.UserId = ug.UserId";
            query += " where qg.UserId = @userId order by qgq.CreateTime desc";

            List<object> listData = this._connection.Query<object>(query, new { userId }, transaction ).ToList();
            return listData;
        }

        public object GetListUserQueueReceive(string userId, IDbTransaction transaction = null)
        {
            string query = "select qgq.*, ug.UserId, ug.Name";
            query += " from [queue_give_quest] qgq left join [queue_receive] qr on qgq.QueueReceiveId = qr.QueueReceiveId";
            query += " left join [queue_give] qg on qgq.QueueGiveId = qg.QueueGiveId";
            query += " left join [user] ug on qg.UserId = ug.UserId";
            query += " where qr.UserId = @userId order by qgq.CreateTime desc";


            List<object> listData = this._connection.Query<object>(query, new { userId }, transaction).ToList();
            return listData;
        }

    }
}