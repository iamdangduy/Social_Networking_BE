﻿using Dapper;
using GiveAndReceive.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace GiveAndReceive.Services
{
    public class UserBankInfoService : BaseService
    {
        public UserBankInfoService() : base() { }
        public UserBankInfoService(IDbConnection db) : base(db) { }

        public List<object> GetListUserBankInfo(string UserId)
        {
            string query = "select * from [user_bank_info] where UserId = @UserId";
            return this._connection.Query<object>(query, new { UserId }).ToList();
        }

        public UserBankInfo GetUserBankInfoById(string UserBankInfoId, IDbTransaction transaction = null)
        {
            string query = "select * from [user_bank_info] where UserBankInfoId = @UserBankInfoId";
            return this._connection.Query<UserBankInfo>(query, new { UserBankInfoId }, transaction).FirstOrDefault();
        }

        public void InsertUserBankInfo(UserBankInfo model, IDbTransaction transaction = null)
        {
            string query = "insert into [user_bank_info] ([UserBankInfoId], [UserId], [BankName], [BankOwnerName], [BankNumber], [QRImage], [IsDefault]) values (@UserBankInfoId, @UserId, @BankName, @BankOwnerName, @BankNumber, @QRImage, @IsDefault)";
            int Status = this._connection.Execute(query, model, transaction);
            if (Status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void UpdateUserBankInfo(UserBankInfo model, IDbTransaction transaction = null)
        {
            string query = "update [user_bank_info] set [BankName] = @BankName, [BankOwnerName] = @BankOwnerName, [BankNumber] = @BankNumber, [QRImage] = @QRImage, [IsDefault] = @IsDefault where UserBankInfoId = @UserBankInfoId";
            int Status = this._connection.Execute(query, model, transaction);
            if (Status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void DeleteUserBankInfo(string UserBankInfoId, IDbTransaction transaction = null)
        {
            string query = "delete from [user_bank_info] where UserBankInfoId = @UserBankInfoId";
            int Status = this._connection.Execute(query, new { UserBankInfoId }, transaction);
            if (Status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
    }
}