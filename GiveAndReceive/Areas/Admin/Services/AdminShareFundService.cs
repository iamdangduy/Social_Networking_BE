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
    public class AdminShareFundService : BaseService
    {
        public AdminShareFundService() : base() { }
        public AdminShareFundService(IDbConnection db) : base(db) { }

        public ShareFund GetBalanceShareFund()
        {
            string query = "SELECT TOP (1) [ShareFundId], [Balance] FROM [GiveAndReceive].[dbo].[share_fund]";
            return this._connection.Query<ShareFund>(query).FirstOrDefault();
        }
    }
}