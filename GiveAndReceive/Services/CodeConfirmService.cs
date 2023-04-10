﻿using Dapper;
using GiveAndReceive.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace GiveAndReceive.Services
{
    public class CodeConfirmService : BaseService
    {
        public CodeConfirmService() : base() { }
        public CodeConfirmService(IDbConnection db) : base(db) { }
        public bool InsertCodeConfirm(CodeConfirm codeConfirm, IDbTransaction transaction = null)
        {
            string insert = "INSERT INTO [dbo].[code_confirm] ([CodeConfirmId],[Phone],[Email],[Code],[ExpiryTime],[CreateTime]) VALUES (@CodeConfirmId,@Phone,@Email,@Code,DATEADD(mi,5,GETDATE()),GETDATE())";
            int status = this._connection.Execute(insert, codeConfirm, transaction);
            return status > 0;
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
    }
}