using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

        public User GetUserById(string id, IDbTransaction transaction = null)
        {
            string query = "select * from [user] where UserId = @id";
            return this._connection.Query<User>(query, new { id }, transaction).FirstOrDefault();
        }

        public User GetUserByAccount(string account, IDbTransaction transaction = null)
        {
            string query = "select top 1 * from [user] where Account=@account";
            return this._connection.Query<User>(query, new { account }, transaction).FirstOrDefault();
        }

        public User GetUserByPhone(string phone, IDbTransaction transaction = null)
        {
            string query = "select * from [user] where Phone = @Phone";
            return this._connection.Query<User>(query, new { Phone = phone }, transaction).FirstOrDefault();
        }
        public User GetUserByEmail(string email, IDbTransaction transaction = null)
        {
            string query = "select * from [user] where Email = @email";
            return this._connection.Query<User>(query, new { email }, transaction).FirstOrDefault();
        }

        public User GetUserByToken(string Token, IDbTransaction transaction = null)
        {
            string query = "select u.* from [user] u join [user_token] ut on u.UserId = ut.UserId where ut.Token = @Token";
            return this._connection.Query<User>(query, new { Token }, transaction).FirstOrDefault();
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
            string query = "UPDATE [dbo].[user] SET [Name]=@Name,[Avatar]=@Avatar,[Account]=@Account,[Phone]=@Phone,[Email]=@Email,[Phone2]=@Phone2,[Address]=@Address WHERE [UserId]=@UserId";
            int status = this._connection.Execute(query, model, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void ChangePassword(string userId, string newPassword, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[user] SET [Password]=@newPassword WHERE [UserId]=@userId";
            int status = this._connection.Execute(query, new { userId, newPassword }, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public bool CheckUserPhoneExist(string phone, string userId, IDbTransaction transaction = null)
        {
            string query = "select count(*) from [user] where Phone = @phone and Phone <> '' and UserId <> @userId";
            int count = this._connection.Query<int>(query, new { phone, userId }, transaction).FirstOrDefault();
            return count > 0;
        }

        public void CheckAccountExist(string account, string userId, IDbTransaction transaction = null)
        {
            string query = "select count(*) from [user] where Account=@account and Account <> '' and UserId <> @userId";
            int count = this._connection.Query<int>(query, new { account, userId }, transaction).FirstOrDefault();
            if (count > 0) throw new Exception("Account đã tồn tại.");
        }

        public void CheckEmailExist(string email, string userId, IDbTransaction transaction = null)
        {
            string query = "select count(*) from [user] where Email=@email and Email <> '' and UserId <> @userId";
            int count = this._connection.Query<int>(query, new { email, userId }, transaction).FirstOrDefault();
            if (count > 0) throw new Exception("Email đã tồn tại.");
        }

        public bool CheckPhoneExist(string phone, IDbTransaction transaction = null)
        {
            string query = "select count(*) from [user] where Phone = @phone and Phone <> '' ";
            int count = this._connection.Query<int>(query, new { phone }, transaction).FirstOrDefault();
            return count > 0;
        }

        public void InsertUser(User user, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[user] ([UserId],[Name],[Avatar],[Account],[Email],[Phone],[Password],[CreateTime])" +
                " VALUES (@UserId, @Name, @Avatar, @Account, @Email, @Phone, @Password, @CreateTime)";
            int status = this._connection.Execute(query, user, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public User GetUserByShareCode(string code, IDbTransaction transaction = null)
        {
            string query = "select TOP(1)* from [user] where ShareCode = @code";
            return this._connection.Query<User>(query, new { code }, transaction).FirstOrDefault();
        }

        public User GetUserByEmailOrPhoneOrAccount(string account, IDbTransaction transaction = null)
        {

            string phone = "";

            if (account.Length > 9) phone = "+84 " + account.Substring(account.Length - 9, 9);
            string query = "select top 1 * from [user] where Phone = @phone or Account=@account";
            return this._connection.Query<User>(query, new { account, phone }, transaction).FirstOrDefault();
        }

        public void UpdateUserToken(string userId, string token, IDbTransaction transaction = null)
        {
            string query = "update [user_token] set Token=@token where UserId=@userId";
            int status = this._connection.Execute(query, new { token = token, userId = userId }, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public UserToken GetUserToken(string token, IDbTransaction transaction = null)
        {
            string query = "select top 1 * from [user_token] where Token=@token";
            return this._connection.Query<UserToken>(query, new { token }, transaction).FirstOrDefault();
        }

        public void InsertUserToken(UserToken model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[user_token]([UserTokenId],[UserId],[Token],[ExpireTime],[CreateTime]) VALUES (@UserTokenId,@UserId,@Token,@ExpireTime,@CreateTime)";
            int status = this._connection.Execute(query, model, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void RemoveUserToken(string token, IDbTransaction transaction = null)
        {
            string query = "update [user_token] set Token=NULL where Token=@token";
            int status = this._connection.Execute(query, new { token = token }, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public List<User> GetListUserByShareCode(string code, IDbTransaction transaction = null)
        {
            string query = "select Name, Account, Avatar, Phone, Email, Phone2, Address from [user] where ParentCode=@code";
            return this._connection.Query<User>(query, new { code = code }, transaction).ToList();
        }

        public void UpdateUserCode(User user, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[user] SET [ParentCode] = @ParentCode, [ShareCode] = @ShareCode, [Depth] = @Depth WHERE [UserId] = @UserId";
            int status = this._connection.Execute(query, user, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public List<User> GetListUserTransferPin(string keyword, IDbTransaction transaction = null)
        {
            string query = "select UserId, Name, Account, Phone, Email from [user] where 1=1 and ShareCode <> ''";
            if(!string.IsNullOrEmpty(keyword))
            {
                keyword = "%" + keyword.Replace(" ","%") + "%";
                query += " and (Account like @keyword or Email like @keyword)";
            }
            return this._connection.Query<User>(query, new { keyword }, transaction).ToList();
        }
    }
}
