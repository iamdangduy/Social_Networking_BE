using Dapper;
using GiveAndReceive.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace GiveAndReceive.Services
{
    public class UserWalletService : BaseService
    {
        public UserWalletService() : base() { }
        public UserWalletService(IDbConnection db) : base(db) { }

        public long GetBalanceByUserId(string UserId, IDbTransaction transaction = null)
        {
            string query = "select Balance from [user_wallet] where UserId = @UserId";
            return this._connection.Query<long>(query, new { UserId }, transaction).FirstOrDefault();
        }

        public UserWallet GetUserWalletByUser(string userId, IDbTransaction transaction = null)
        {
            string query = "select * from [user_wallet] where UserId = @userId";
            return this._connection.Query<UserWallet>(query, new { userId }, transaction).FirstOrDefault(); 
        }
    }
}