using Dapper;
using GiveAndReceive.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace GiveAndReceive.Services
{
    public class UserTransactionService : BaseService
    {
        public UserTransactionService() : base() { }
        public UserTransactionService(IDbConnection db) : base(db) { }

        public List<UserTransaction> GetListUserTransactionByUser(string userId, IDbTransaction transaction = null)
        {
            string query = "select * from [dbo].[user_transaction] where UserId = @userId order by CreateTime desc";
            return this._connection.Query<UserTransaction>(query, new { userId }, transaction).ToList();
        }

        public void InsertUserTransaction(UserTransaction model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[user_transaction] ([UserTransactionId],[UserId],[Amount],[Note],[CreateTime])" +
                " VALUES (@UserTransactionId,@UserId,@Amount,@Note,@CreateTime)";
            int status = this._connection.Execute(query, model, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
    }
}