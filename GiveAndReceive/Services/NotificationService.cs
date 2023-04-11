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
    }
}