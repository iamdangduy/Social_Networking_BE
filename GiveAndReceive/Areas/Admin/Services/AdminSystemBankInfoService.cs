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
    public class AdminSystemBankInfoService : BaseService
    {
        public AdminSystemBankInfoService() : base() { }
        public AdminSystemBankInfoService(IDbConnection db) : base(db) { }

        public void InsertSystemBankInfo(SystemBankInfo model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[system_bank_info]([SystemBankInfoId],[BankName],[BankOwnerName],[BankNumber],[IsDefault]) VALUES (@SystemBankInfoId,@BankName,@BankOwnerName,@BankNumber,@IsDefault)";
            int status = this._connection.Execute(query, model, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
        public void UpdateSystemBankInfo(SystemBankInfo model, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[system_bank_info] SET [SystemBankInfoId] = @SystemBankInfoId,[BankName] = @BankName,[BankOwnerName] = @BankOwnerName,[BankNumber] = @BankNumber,[IsDefault] = @IsDefault WHERE SystemBankInfoId = @SystemBankInfoId";
            int status = this._connection.Execute(query, model, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
        public void DeleteSystemBankInfo(string SystemBankInfoId, IDbTransaction transaction = null)
        {
            string query = "Delete [system_bank_info] where SystemBankInfoId = @SystemBankInfoId";
            int status = this._connection.Execute(query, new { SystemBankInfoId }, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
        public SystemBankInfo GetSystemBankInfoById(string SystemBankInfoId, IDbTransaction transaction = null)
        {
            string query = "select TOP(1)* from [system_bank_info] where SystemBankInfoId = @SystemBankInfoId";
            return this._connection.Query<SystemBankInfo>(query, new { SystemBankInfoId }, transaction).FirstOrDefault();
        }
        public List<SystemBankInfo> GetListAllSystemBankInfo(IDbTransaction transaction = null)
        {
            string query = "select * from [system_bank_info]";
            return this._connection.Query<SystemBankInfo>(query, null, transaction).ToList();
        }
        public SystemBankInfo GetSystemBankInfoIsDefault(IDbTransaction transaction = null)
        {
            string query = "select TOP(1)* from system_bank_info where IsDefault=1";
            return this._connection.Query<SystemBankInfo>(query, null, transaction).FirstOrDefault();
        }

        public void SetSystemBankInfoIsDefaultFalse(string SystemBankInfoId, IDbTransaction transaction = null)
        {
            string query = "Update system_bank_info SET IsDefault=0 where SystemBankInfoId=@SystemBankInfoId";
            int status = this._connection.Execute(query, new { SystemBankInfoId }, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
    }
}