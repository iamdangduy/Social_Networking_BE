using GiveAndReceive.Filters;
using GiveAndReceive.Models;
using GiveAndReceive.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GiveAndReceive.ApiControllers
{
    public class UserTransactionController : ApiBaseController
    {
        [HttpGet]
        public JsonResult GetListUserTransactionByUser()
        {
            try
            {
                string token = Request.Headers.Authorization.ToString();
                UserService userService = new UserService();
                User user = userService.GetUserByToken(token);
                if (user == null) return Unauthorized();

                UserTransactionService userTransactionService = new UserTransactionService();
                return Success(userTransactionService.GetListUserTransactionByUser(user.UserId));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}