using Dapper;
using GiveAndReceive.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace GiveAndReceive.Services
{
    public class UserPropertiesService : BaseService
    {
        public UserPropertiesService() : base() { }
        public UserPropertiesService(IDbConnection db) : base(db) { }

        public void CreateUserProperties(UserProperties model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[user_properties]([UserId],[RankId],[CitizenIdentificationName],[CitizenIdentificationNumber],[CitizenIdentificationPlaceOf],[CitizenIdentificationDateOf]," +
                "[CitizenIdentificationAddress],[CitizenIdentificationImageFront],[CitizenIdentificationImageBack],[PhotoFace],[IdentificationApprove],[TotalAmountGive],[TotalAmountReceive]) VALUES " +
                "(@UserId,@RankId,@CitizenIdentificationName,@CitizenIdentificationNumber,@CitizenIdentificationPlaceOf,@CitizenIdentificationDateOf,@CitizenIdentificationAddress,@CitizenIdentificationImageFront," +
                "@CitizenIdentificationImageBack,@PhotoFace,@IdentificationApprove,@TotalAmountGive,@TotalAmountReceive)";
            int status = this._connection.Execute(query, model, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
    }
}