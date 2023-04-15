using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using System.Data;
using GiveAndReceive.Models;
namespace GiveAndReceive.Services
{
    public class SystemConfigService : BaseService 
    {
        public SystemConfigService() : base() { }
        public SystemConfigService(IDbConnection db) : base(db) { }
        public SystemConfig GetSystemConfig(string id,IDbTransaction transaction = null)
        {
            string query = "select * from system_config where SystemConfigId=@id";
            return this._connection.Query<SystemConfig>(query, new { id }, transaction).FirstOrDefault();
        }
    }
}