using Dapper;
using GiveAndReceive.Models;
using GiveAndReceive.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace GiveAndReceive.Areas.Admin.Services
{
    public class AdminUserWalletService : BaseService
    {
        public AdminUserWalletService() : base() { }
        public AdminUserWalletService(IDbConnection db) : base(db) { }

        public void UpdateBalanceByUserId (string userId, long balance, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[user_wallet] SET [Balance] = [Balance] + @balance WHERE [UserId] = @userId";
            int status = this._connection.Execute(query, new {userId, balance}, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
    }
}