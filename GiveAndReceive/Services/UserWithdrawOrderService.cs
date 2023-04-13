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

        public List<UserWithdrawOrder> GetListWithdrawPendingByUser (string userId, IDbTransaction transaction = null)
        {
            string query = "select * from [dbo].[user_withdraw_order] where [Status] = 'PENDING' and UserId = @userId";
            return this._connection.Query<UserWithdrawOrder>(query, new { userId }, transaction).ToList();
        }

        public List<UserWithdrawOrder> GetListWithdrawStatusByUser(string userId, string status, IDbTransaction transaction = null)
        {
            string query = "select * from [dbo].[user_withdraw_order] where [Status] = @status and UserId = @userId";
            return this._connection.Query<UserWithdrawOrder>(query, new { status, userId }, transaction).ToList();
        }

        public UserWithdrawOrder GetUserWithdrawOrderById(string id, IDbTransaction transaction = null)
        {
            string query = "select * from [dbo].[user_withdraw_order] where UserWithdrawOrderId = @id";
            return this._connection.Query<UserWithdrawOrder>(query, new { id }, transaction).FirstOrDefault();
        }

        public void InsertUserWithdrawOrder(UserWithdrawOrder model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[user_withdraw_order] ([UserWithdrawOrderId],[UserId],[Code],[Amount],[Status],[CreateTime]) " +
                "VALUES (@UserWithdrawOrderId,@UserId,@Code,@Amount,@Status,@CreateTime)";
            int status = this._connection.Execute(query, model, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void UpdateUserWithdrawOrderStatus(UserWithdrawOrder model, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[user_withdraw_order] SET [Status] = @Status WHERE [UserWithdrawOrderId] = @UserWithdrawOrderId";
            int status = this._connection.Execute(query, model, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
        #endregion

        #region UserWithdrawOrderStatus
        public void InsertUserWithdrawOrderStatus(UserWithdrawOrderStatus model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[user_withdraw_order_status] ([UserWithdrawOrderStatusId],[UserWithdrawOrderId],[Status],[CreateTime]) " +
                "VALUES (@UserWithdrawOrderStatusId,@UserWithdrawOrderId,@Status,@CreateTime)";
            int status = this._connection.Execute(query, model, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
        #endregion
    }
}