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
    public class AdminSystemWalletService : BaseService
    {
        public AdminSystemWalletService() : base() { }
        public AdminSystemWalletService(IDbConnection db) : base(db) { }

        public long GetBalance()
        {
            string query = "select Balance from [system_wallet] where [SystemWalletId] = 'CASH'";
            return this._connection.Query<long>(query).FirstOrDefault();
        }
    }
}