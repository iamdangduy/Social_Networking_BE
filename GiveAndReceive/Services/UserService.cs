using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Web;
using Dapper;
using GiveAndReceive.Models;

namespace GiveAndReceive.Services
{
    public class UserService : BaseService
    {
        public UserService() : base() { }
        public UserService(IDbConnection db) : base(db) { }
        public User GetUserByToken(string Token, IDbTransaction transaction = null)
        {
            string query = "select u.* from [user] u join [user_token] ut on u.UserId = ut.UserId where ut.Token = @Token";
            return this._connection.Query<User>(query, new { Token }, transaction).FirstOrDefault();
        }

        public User GetUserById(string userId, IDbTransaction transaction = null)
        {
            string query = "select top 1 * from [user] where UserId=@userId";
            return this._connection.Query<User>(query, new { userId }, transaction).FirstOrDefault();
        }

        public void UpdateUserPoint(string userId, decimal point, IDbTransaction transaction = null)
        {
            string query = "update [user_wallet] set Point=Point+@point where UserId=@userId";
            int status = this._connection.Execute(query, new { userId, point }, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public UserWallet GetUserWallet(string userId, IDbTransaction transaction = null)
        {
            string query = "select top 1 * from [user_wallet] where UserId = @userId";
            return this._connection.Query<UserWallet>(query, new { userId }, transaction).FirstOrDefault();
        }

        public void InsertUserTransaction(UserTransaction model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[user_transaction] ([UserTransactionId],[UserId],[Amount],[Note],[CreateTime]) VALUES (@UserTransactionId,@UserId,@Amount,@Note,@CreateTime)";
            int status = this._connection.Execute(query, model, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
        
        public void UpdateUser(User model, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[user] SET [Name]=@Name,[Avatar]=@Avatar,[Account]=@Account,[Phone]=@Phone,[Email]=@Email WHERE [UserId]=@UserId";
            int status = this._connection.Execute(query, model, transaction);
            if (status > 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void ChangePassword(string userId, string newPassword, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[user] SET [Password]=@newPassword WHERE [UserId]=@userId";
            int status = this._connection.Execute(query, new {userId, newPassword}, transaction);
            if(status > 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
        public bool CheckPhoneExist(string phone, IDbTransaction transaction = null)
        {
            string query = "select count(*) from [user] where Phone = @phone and Phone <> '' ";
            int count = this._connection.Query<int>(query, new { phone }, transaction).FirstOrDefault();
            return count > 0;
        }
        public void InsertUser(User user, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[user] ([UserId],[Name],[Avatar],[Account],[Email],[Phone],[Password],[ShareCode],[ParentCode],[CreateTime])" +
                " VALUES (@UserId, @Name, @Avatar, @Account, @Email, @Phone, @Password, @ShareCode, @ParentCode, @CreateTime)";
            int status = this._connection.Execute(query, user, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
        public User GetUserByShareCode(string code, IDbTransaction transaction = null)
        {
            string query = "select TOP(1)* from [user] where ShareCode = @code";
            return this._connection.Query<User>(query, new { code }, transaction).FirstOrDefault();
        }
    }
}
