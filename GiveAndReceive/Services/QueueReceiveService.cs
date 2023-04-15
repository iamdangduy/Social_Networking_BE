using Dapper;
using GiveAndReceive.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace GiveAndReceive.Services
{
    public class QueueReceiveService : BaseService
    {
        public QueueReceiveService() : base() { }
        public QueueReceiveService(IDbConnection connection) : base(connection) { }
        public QueueReceive GetQueueReceiveByUserId(string userId, IDbTransaction transaction = null)
        {
            string query = "select TOP(1)* from [queue_receive] where UserId=@userId order by CreateTime desc";
            return this._connection.Query<QueueReceive>(query, new { userId }, transaction).FirstOrDefault();
        }

        public void Insert (QueueReceive queueReceive, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[queue_receive] ([QueueReceiveId],[UserId],[Status],[TotalReceivedAmount],[TotalExpectReceiveAmount],[CreateTime]) VALUES (@QueueReceiveId,@UserId,@Status,@TotalReceivedAmount,@TotalExpectReceiveAmount,@CreateTime)";
            int status = this._connection.Execute(query, queueReceive, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
    }
}