using GiveAndReceive.Filters;
using GiveAndReceive.Models;
using GiveAndReceive.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace GiveAndReceive.ApiControllers
{
    public class NotificationController : ApiBaseController
    {
        [HttpGet]
        public JsonResult GetListNotification(int page)
        {
            try
            {
                UserService userService = new UserService();
                string token = Request.Headers.Authorization.ToString();
                User user = userService.GetUserByToken(token);
                if (user == null) return Unauthorized();
                NotificationService notificationService = new NotificationService();
                if (page <= 0) page = 1;
                return Success(notificationService.GetListNotification(user.UserId, page));

            }
            catch (Exception ex)
            {
                return Error();
            }
        }

        [HttpGet]
        public JsonResult UpdateNotificationRead(string id)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        string token = Request.Headers.Authorization.ToString();
                        UserService userService = new UserService(connect);
                        User user = userService.GetUserByToken(token, transaction);
                        if (user == null) return Unauthorized();
                        NotificationService notificationService = new NotificationService(connect);
                        Notification notification = notificationService.GetNotificationById(id, transaction);
                        notification.IsRead = true;
                        notificationService.UpdateNotificationRead(notification, transaction);
                        transaction.Commit();
                        return Success();
                    }
                }

            }
            catch (Exception ex)
            {
                return Error();
            }
        }
    }
}
