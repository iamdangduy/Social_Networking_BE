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
    public class AdminSystemConfigService : BaseService
    {
        public AdminSystemConfigService() : base() { }
        public AdminSystemConfigService(IDbConnection db) : base(db) { }

        public List<SystemConfig> GetListConfig(IDbTransaction transaction = null)
        {
            string query = "Select * from system_config";
            return this._connection.Query<SystemConfig>(query, transaction).ToList();
        }

        public bool UpdateConfig(SystemConfig model, IDbTransaction transaction = null)
        {
            string query = "update system_config set Value = @Value where SystemConfigId = @SystemConfigId";
            var status = this._connection.Execute(query, model, transaction);
            return status > 0;
        }

        public SystemConfig GetSystemConfigById(string id, IDbTransaction transaction = null)
        {
            string query = "select * from [dbo].[system_config] where SystemConfigId=@id";
            return this._connection.Query<SystemConfig>(query, new { id }, transaction).FirstOrDefault();
        }
    }
}