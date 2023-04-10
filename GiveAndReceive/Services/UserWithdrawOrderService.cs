using Dapper;
using GiveAndReceive.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace GiveAndReceive.Services
{
    public class UserWithdrawOrderService : BaseService
    {
        public UserWithdrawOrderService() : base() { }
        public UserWithdrawOrderService(IDbConnection db) : base(db) { }
        #region UserWithdrawOrder
        public List<UserWithdrawOrder> GetListUserWithdrawOrder (IDbTransaction transaction = null)
        {
            string query = "select * from [dbo].[user_withdraw_order]";
            return this._connection.Query<UserWithdrawOrder> (query, transaction).ToList();
        }
        #endregion

        #region UserWithdrawOrderStatus
        #endregion
    }
}