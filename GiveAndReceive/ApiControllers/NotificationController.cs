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
        public JsonResult GetListNotification()
        {
            try
            {
                UserService userService = new UserService();
                string token = Request.Headers.Authorization.ToString();
                User user = userService.GetUserByToken(token);
                if (user == null) return Unauthorized();

                NotificationService notificationService = new NotificationService();
                return Success(notificationService.GetListNotification(user.UserId), "Lấy dữ liệu thành công!");

            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult UpdateNotificationRead(string NotificationId)
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
                        Notification notification = notificationService.GetNotificationById(NotificationId, transaction);
                        notification.IsRead = true;
                        notificationService.UpdateNotificationRead(notification, transaction);
                        transaction.Commit();
                        return Success();
                    }
                }

            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetNotificationIsNotRead()
        {
            try
            {
                string token = Request.Headers.Authorization.ToString();
                UserService userService = new UserService();
                User user = userService.GetUserByToken(token);
                if (user == null) return Unauthorized();

                NotificationService notificationService = new NotificationService();
                return Success(notificationService.GetNotificationIsNotRead(user.UserId), "Lấy dữ liệu thành công!");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

    }
}
