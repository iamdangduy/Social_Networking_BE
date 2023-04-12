using Dapper;
using GiveAndReceive.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using static GiveAndReceive.Models.JsonResult;

namespace GiveAndReceive.Services
{
    public class NotificationService : BaseService
    {
        public NotificationService() : base() { }
        public NotificationService(IDbConnection db) : base(db) { }
        public List<Notification> GetListNotification(string userId, int page)
        {

            string query = "select * from notification where 1 = 1 and UserId=@userId order by NotificationId desc";
            query += " OFFSET(" + (page - 1) * Constant.USER_PAGE_SIZE + ") ROWS FETCH NEXT(" + Constant.USER_PAGE_SIZE + ") ROWS ONLY ";
            List<Notification> ls = this._connection.Query<Notification>(query, new { userId = userId }).ToList();
            return ls;
        }

        public Notification GetNotificationById(string id, IDbTransaction transaction = null)
        {
            string query = "select * from notification where NotificationId = @id";
            return this._connection.Query<Notification>(query, new { id }, transaction).FirstOrDefault();
        }

        public void UpdateNotificationRead(Notification model, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[notification] SET IsRead = @IsRead WHERE NotificationId = @NotificationId";
            int status = this._connection.Execute(query, model, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
    }
}