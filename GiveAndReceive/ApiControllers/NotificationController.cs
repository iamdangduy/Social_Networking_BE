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
        [ApiTokenRequire]
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
    }
}
