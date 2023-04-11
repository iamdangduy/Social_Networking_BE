using Dapper;
using GiveAndReceive.Models;
using GiveAndReceive.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace GiveAndReceive.Areas.Admin.Services
{
    public class AdminUserTransactionService : BaseService
    { 
        public AdminUserTransactionService() : base() { }
        public AdminUserTransactionService(IDbConnection db) : base(db) { }

        public void InsertUserTransaction(UserTransaction model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[user_transaction] ([UserTransactionId],[UserId],[Amount],[Note],[CreateTime])" +
                " VALUES (@UserTransactionId,@UserId,@Amount,@Note,@CreateTime)";
            int status = this._connection.Execute(query, model, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
    }
}