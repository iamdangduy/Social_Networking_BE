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
            string query = "select * from [dbo].[user_transaction] where UserId = @userId";
            return this._connection.Query<UserTransaction>(query, new { userId }, transaction).ToList();
        }
    }
}