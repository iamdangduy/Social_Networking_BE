using Dapper;
using GiveAndReceive.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace GiveAndReceive.Services
{
    public class UserWareHouseService : BaseService
    {
        public UserWareHouseService() : base() { }
        public UserWareHouseService(IDbConnection db) : base(db) { }

        #region UserWareHouse
        public UserWareHouse GetWareHouseByUserId(string userId, IDbTransaction transaction = null)
        {
            string query = "select * from [user_ware_house] where [UserId] = @userId";
            return this._connection.Query<UserWareHouse>(query, new { userId }, transaction).FirstOrDefault();
        }

        public void InsertUserWareHouse (UserWareHouse model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[user_ware_house] ([UserId],[TotalProduct]) VALUES (@UserId,@TotalProduct)";
            int status = this._connection.Execute(query, model, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void UpdateUserWareHouse(UserWareHouse model, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[user_ware_house] SET [TotalProduct] = [TotalProduct] + @TotalProduct WHERE [UserId]=@UserId";
            int status = this._connection.Execute(query, model, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
        #endregion

        #region UserWareHouseDetail
        public void InsertUserWareHouseDetail(UserWareHouseDetail model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[user_ware_house_detail] ([UserWareHouseDetailId],[UserId],[ProductId],[Quantity],[Status]) " +
                "VALUES (@UserWareHouseDetailId,@UserId,@ProductId,@Quantity,@Status)";
            int status = this._connection.Execute(query, model, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
        #endregion
    }
}