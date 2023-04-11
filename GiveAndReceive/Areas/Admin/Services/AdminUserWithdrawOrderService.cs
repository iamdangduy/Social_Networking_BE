using Dapper;
using GiveAndReceive.Models;
using GiveAndReceive.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using static GiveAndReceive.Models.JsonResult;

namespace GiveAndReceive.Areas.Admin.Services
{
    public class AdminUserWithdrawOrderService : BaseService
    {
        public AdminUserWithdrawOrderService() : base() { }
        public AdminUserWithdrawOrderService(IDbConnection db) : base(db) { }

        #region UserWithdrawOrder
        public ListUserWithdrawOrderView GetListUserWithdrawOrder(int page, string keyword, IDbTransaction transaction = null)
        {
            ListUserWithdrawOrderView listUserWithdrawOrderView = new ListUserWithdrawOrderView();
            listUserWithdrawOrderView.List = new List<UserWithdrawOrderModel>();
            listUserWithdrawOrderView.TotalPage = 0;

            string querySelect = "select uwo.UserWithdrawOrderId, u.UserId, u.Name, u.Phone, u.Email, uwo.Code, uwo.Amount, uwo.Status, uwo.CreateTime";
            string queryCount = "select count(*)";
            string query = " from [dbo].[user_withdraw_order] uwo left join [dbo].[user] u on uwo.UserId = u.UserId where 1=1";

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = "%" + keyword.Replace(' ', '%') + "%";
                query += " and (u.Phone like @keyword or u.Email like @keyword)";
            }

            int totalRow = _connection.Query<int>(queryCount + query, new { keyword = keyword }, transaction).FirstOrDefault();
            if (totalRow > 0)
            {
                listUserWithdrawOrderView.TotalPage = (int)Math.Ceiling((decimal)totalRow / Constant.ADMIN_PAGE_SIZE);
            }

            int skip = (page - 1) * Constant.ADMIN_PAGE_SIZE;
            query += " order by uwo.CreateTime desc offset " + skip + " rows fetch next " + Constant.ADMIN_PAGE_SIZE + " rows only";
            listUserWithdrawOrderView.List = this._connection.Query<UserWithdrawOrderModel>(querySelect + query, new { keyword = keyword }, transaction).ToList();
            return listUserWithdrawOrderView;
        }

        public UserWithdrawOrder GetUserWithdrawOrderById (string id, IDbTransaction transaction = null)
        {
            string query = "select * from [dbo].[user_withdraw_order] where UserWithdrawOrderId = @id";
            return this._connection.Query<UserWithdrawOrder>(query, new {id}, transaction).FirstOrDefault();
        }

        public void InsertUserWithdrawOrder (UserWithdrawOrder model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[user_withdraw_order] ([UserWithdrawOrderId],[UserId],[Code],[Amount],[Status],[CreateTime]) " +
                "VALUES (@UserWithdrawOrderId,@UserId,@Code,@Amount,@Status,@CreateTime)";
            int status = this._connection.Execute(query, model, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void UpdateUserWithdrawOrderStatus (UserWithdrawOrder model, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[user_withdraw_order] SET [Status] = @Status WHERE [UserWithdrawOrderId] = @UserWithdrawOrderId";
            int status = this._connection.Execute(query, model, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        #endregion

        #region UserWithdrawOrderStatus
        public void InsertUserWithdrawOrderStatus (UserWithdrawOrderStatus model, IDbTransaction transaction=null)
        {
            string query = "INSERT INTO [dbo].[user_withdraw_order_status] ([UserWithdrawOrderStatusId],[UserWithdrawOrderId],[Status],[CreateTime]) " +
                "VALUES (@UserWithdrawOrderStatusId,@UserWithdrawOrderId,@Status,@CreateTime)";
            int status = this._connection.Execute(query, model, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
        #endregion
    }
}