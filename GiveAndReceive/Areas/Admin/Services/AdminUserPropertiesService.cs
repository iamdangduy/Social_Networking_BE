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
    public class AdminUserPropertiesService : BaseService
    {
        public AdminUserPropertiesService() : base() { }
        public AdminUserPropertiesService(IDbConnection db) : base(db) { }

        public UserProperties GetUserPropertiesByUserId(string UserId, IDbTransaction transaction = null)
        {
            string query = "select * from [user_properties] where UserId = @UserId";
            return this._connection.Query<UserProperties>(query, new { UserId }, transaction).FirstOrDefault();
        }

        public void UpdateStatusUserIdentity(string UserId, bool Status, IDbTransaction transaction = null)
        {
            string query = "update [user_properties] set IdentificationApprove = @Status where UserId = @UserId";
            int numberOfEffectedRows = this._connection.Execute(query, new { UserId, Status }, transaction);
            if (numberOfEffectedRows <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
    }
}