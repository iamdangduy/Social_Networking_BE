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
            string query = "select * from [user_pin_transfer] where UserGiveId = @userId or UserReceiveId = @userId";
            return this._connection.Query<UserPinTransfer> (query, new {userId}, transaction).ToList();
        }

        public void InsertUserPinTransfer(UserPinTransfer userPinTransfer, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[user_pin_transfer] ([UserPinTransferId],[UserGiveId],[UserReceiveId],[Pin],[Status],[CreateTime],[Message]) " +
                "VALUES (@UserPinTransferId,@UserGiveId,@UserReceiveId,@Pin,@Status,@CreateTime,@Message)";
            int status = this._connection.Execute(query, userPinTransfer, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

        }
    }
}