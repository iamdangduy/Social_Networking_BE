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
            string query = "INSERT INTO [dbo].[user_properties] ([UserId], [RankId], [CitizenIdentificationName], [CitizenIdentificationNumber], [CitizenIdentificationPlaceOf], [CitizenIdentificationDateOf], " +
                "[CitizenIdentificationAddress], [CitizenIdentificationImageFront], [CitizenIdentificationImageBack], [PhotoFace], [IdentificationApprove], [Status], [TotalAmountGive], [TotalAmountReceive] ) VALUES " +
                "(@UserId, @RankId, @CitizenIdentificationName, @CitizenIdentificationNumber, @CitizenIdentificationPlaceOf, @CitizenIdentificationDateOf, @CitizenIdentificationAddress, @CitizenIdentificationImageFront," +
                "@CitizenIdentificationImageBack, @PhotoFace, @IdentificationApprove, @Status, @TotalAmountGive, @TotalAmountReceive)";
            int status = this._connection.Execute(query, model, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public UserProperties GetUserPropertiesByUserId(string userId, IDbTransaction transaction = null)
        {
            string query = "select top(1) * from user_properties where UserId =@userId";
            return this._connection.Query<UserProperties>(query, new { userId }, transaction).FirstOrDefault();
        }

        public void CreateUserPropertiesForIdentity(UserProperties model, IDbTransaction transaction = null)
        {
            string query = "update [user_properties] " +
                "set CitizenIdentificationImageFront = @CitizenIdentificationImageFront, " +
                "CitizenIdentificationImageBack = @CitizenIdentificationImageBack, " +
                "PhotoFace = @PhotoFace, " +
                "CitizenIdentificationName = @CitizenIdentificationName, " +
                "CitizenIdentificationNumber = @CitizenIdentificationNumber, " +
                "CitizenIdentificationPlaceOf = @CitizenIdentificationPlaceOf, " +
                "CitizenIdentificationDateOf = @CitizenIdentificationDateOf, " +
                "CitizenIdentificationAddress = @CitizenIdentificationAddress, " +
                "IdentificationApprove = 0, " +
                "Status = @Status " +
                "where UserId = @UserId";
            int Status = this._connection.Execute(query, model, transaction);
            if (Status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void UpdateUserPropertiesForIdentity(UserProperties model, IDbTransaction transaction = null)
        {
            string query = $"update [user_properties] set IdentityImageFront = @IdentityImageFront, IdentityImageBack = @IdentityImageBack, IdentityImagePortrait = @IdentityImagePortrait, IdentityFullName = @IdentityFullName, IdentityNumber = @IdentityNumber, IdentityBirthDate = @IdentityBirthDate, IdentityAddress = @IdentityAddress, IdentityDateOf = @IdentityDateOf, IdentityPlaceOf = @IdentityPlaceOf, IdentityApprove = 0, Status = @Status where UserId = @UserId";
            int Status = this._connection.Execute(query, model, transaction);
            if (Status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public string CheckExistIdentity(string CitizenIdentificationNumber, IDbTransaction transaction = null)
        {
            string query = "select * from [user_properties] where CitizenIdentificationNumber = @CitizenIdentificationNumber";
            return this._connection.Query<string>(query, new { CitizenIdentificationNumber }, transaction).FirstOrDefault();
        }

        public long GetTotalAmountGive(string UserId)
        {
            string query = "select TotalAmountGive from [user_properties] where UserId = @UserId";
            return this._connection.Query<long>(query, new { UserId }).FirstOrDefault();
        }

        public long GetTotalAmountReceive(string UserId)
        {
            string query = "select TotalAmountReceive from [user_properties] where UserId = @UserId";
            return this._connection.Query<long>(query, new { UserId }).FirstOrDefault();
        }


    }
}