using Dapper;
using GiveAndReceive.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Policy;
using System.Web;

namespace GiveAndReceive.Services
{
    public class CodeConfirmService : BaseService
    {
        public CodeConfirmService() : base() { }
        public CodeConfirmService(IDbConnection db) : base(db) { }
        public void InsertCodeConfirm(CodeConfirm codeConfirm, IDbTransaction transaction = null)
        {
            string insert = "INSERT INTO [dbo].[code_confirm] ([CodeConfirmId],[Email],[Code],[ExpiryTime],[CreateTime]) VALUES (@CodeConfirmId,@Email,@Code,@ExpiryTime,@CreateTime)";
            int status = this._connection.Execute(insert, codeConfirm, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
        public CodeConfirm GetCodeConfirmByPhone(string Phone, IDbTransaction transaction = null)
        {
            string query = "select top 1 * from code_confirm where Phone = @Phone order by CreateTime desc";
            return this._connection.Query<CodeConfirm>(query, new { Phone = Phone }, transaction).FirstOrDefault();
        }
        public int CountCodeConfirmOfEOPIn24Hours(string phone, IDbTransaction transaction = null)
        {
            string query = "select count(*) from code_confirm where Phone = @phone and CreateTime between DATEADD(hh,-24,GETDATE()) and GETDATE()";
            return this._connection.Query<int>(query, new { phone = phone }, transaction).FirstOrDefault();
        }

        public CodeConfirm GetCodeConfirmByEmail(string Email, IDbTransaction transaction = null)
        {
            string query = "select top 1 * from code_confirm where Email = @Email order by CreateTime desc";
            return this._connection.Query<CodeConfirm>(query, new { Email = Email }, transaction).FirstOrDefault();
        }
    }
}