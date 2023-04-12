using Dapper;
using GiveAndReceive.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace GiveAndReceive.Services
{
    public class UserPinTransferService : BaseService
    {
        public UserPinTransferService() : base() { }
        public UserPinTransferService(IDbConnection db) : base(db) { }
        public List<UserPinTransfer> GetListPinTransferByUser(string userId, IDbTransaction transaction = null)
        {
            string query = "select * from [user_pin_transfer] where UserId = @userId";
            return this._connection.Query<UserPinTransfer> (query, new {userId}, transaction).ToList();
        }
    }
}