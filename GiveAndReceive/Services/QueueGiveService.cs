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

        public void InsertQueueGive(QueueGive queueGive, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[queue_give] ([QueueGiveId],[UserId],[Status],[CreateTime]) VALUES (@QueueGiveId,@UserId,@Status,@CreateTime)";
            int status = this._connection.Execute(query, queueGive, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
    }
}