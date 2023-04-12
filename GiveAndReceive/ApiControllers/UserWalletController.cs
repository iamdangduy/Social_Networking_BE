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
    public class UserWalletController : ApiBaseController
    {
        [HttpGet]
        [ApiTokenRequire]
        public JsonResult GetBalanceByUserId()
        {
            try
            {
                string token = Request.Headers.Authorization.ToString();
                UserService userService = new UserService();
                User user = userService.GetUserByToken(token);
                if (user == null) return Unauthorized();

                UserWalletService userWalletService = new UserWalletService();
                return Success(userWalletService.GetBalanceByUserId(user.UserId), "Lấy dữ liệu thành công!");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        [ApiTokenRequire]
        public JsonResult GetListWithdrawPendingByUser()
        {
            try
            {
                string token = Request.Headers.Authorization.ToString();
                UserService userService= new UserService();
                User user = userService.GetUserByToken(token);
                if(user == null) return Unauthorized();

                UserWithdrawOrderService userWithdrawOrderService = new UserWithdrawOrderService();
                return Success(userWithdrawOrderService.GetListWithdrawPendingByUser(user.UserId));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}
