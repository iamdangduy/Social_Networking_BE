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
    public class AdminNotificationService : BaseService
    {
        public AdminNotificationService() : base() { }
        public AdminNotificationService(IDbConnection db) : base(db) { }

        public bool InsertNotification(Notification model, IDbTransaction transaction = null)
        {
            string query = "insert into notification (NotificationId, UserId, Message, IsRead, CreateTime) values (@NotificationId, @UserId, @Message, @IsRead, @CreateTime)";
            int status = this._connection.Execute(query, model, transaction);
            return status > 0;
        }
    }
}