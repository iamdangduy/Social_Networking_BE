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
            string query = "select TOP(1) * from [queue_receive] where UserId=@userId order by CreateTime desc";
            return this._connection.Query<QueueReceive>(query, new { userId }, transaction).FirstOrDefault();
        }

        public QueueReceive GetQueueReceive(string queueReceiveId, IDbTransaction transaction = null)
        {
            string query = "select TOP(1) * from [queue_receive] where QueueReceiveId=@queueReceiveId";
            return this._connection.Query<QueueReceive>(query, new { queueReceiveId }, transaction).FirstOrDefault();
        }


        public List<QueueReceive> GetListAvaiableQueueReceive(IDbTransaction transaction = null) {
            string query = " select top 2 * from  queue_receive where TotalExpectReceiveAmount < (select cast(Value as bigint) from system_config where SystemConfigId='LIMIT_RECEIVE') order by CreateTime asc";
            return this._connection.Query<QueueReceive>(query, null, transaction).ToList();
        }

        public void UpdateTotalExpectReceiveAmount(string id, long amount, IDbTransaction transaction = null) {
            string query = "update [queue_receive] set TotalExpectReceiveAmount=TotalExpectReceiveAmount+@amount where QueueReceiveId=@id";
            int status = this._connection.Execute(query, new { id,amount }, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public QueueReceive GetRandomBot(IDbTransaction transaction = null) {
            string query = "select top 1 * from [queue_receive] where Status='BOT' order by newid()";
            return this._connection.Query<QueueReceive>(query, null, transaction).FirstOrDefault();
        }

        public void Insert (QueueReceive queueReceive, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[queue_receive] ([QueueReceiveId],[UserId],[Status],[TotalReceivedAmount],[TotalExpectReceiveAmount],[CreateTime]) VALUES (@QueueReceiveId,@UserId,@Status,@TotalReceivedAmount,@TotalExpectReceiveAmount,@CreateTime)";
            int status = this._connection.Execute(query, queueReceive, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

    }
}