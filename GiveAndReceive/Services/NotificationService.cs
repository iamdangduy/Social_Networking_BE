using Dapper;
using GiveAndReceive.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace GiveAndReceive.Services
{
    public class NotificationService : BaseService
    {
        public NotificationService() : base() { }
        public NotificationService(IDbConnection db) : base(db) { }

        public List<Notification> GetListNotification(string userId)
        {

            string query = "select TOP(20) n.* from [notification] n where n.UserId = @userId order by n.CreateTime desc";
            List<Notification> ls = this._connection.Query<Notification>(query, new { userId }).ToList();
            return ls;
        }

        public Notification GetNotificationById(string id, IDbTransaction transaction = null)
        {
            string query = "select * from notification where NotificationId = @id";
            return this._connection.Query<Notification>(query, new { id }, transaction).FirstOrDefault();
        }

        public void CreateNotification(Notification model, IDbTransaction transaction = null) {
            string query = "INSERT INTO [dbo].[notification] ([NotificationId], [UserId], [Message], [IsRead], [Link], [CreateTime], [MessageShort]) VALUES (@NotificationId, @UserId, @Message, @IsRead, @Link, @CreateTime, @MessageShort)";
            int status = this._connection.Execute(query, model, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void UpdateNotificationRead(Notification model, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[notification] SET IsRead = @IsRead WHERE NotificationId = @NotificationId";
            int status = this._connection.Execute(query, model, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public int GetNotificationIsNotRead(string UserId)
        {
            string query = "select COUNT(*) from [notification] where UserId = @UserId and IsRead = 0";
            return this._connection.Query<int>(query, new { UserId }).FirstOrDefault();
        }
    }
}