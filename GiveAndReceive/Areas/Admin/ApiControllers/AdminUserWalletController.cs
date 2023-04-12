using GiveAndReceive.ApiControllers;
using GiveAndReceive.Areas.Admin.Services;
using GiveAndReceive.Filters;
using GiveAndReceive.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GiveAndReceive.Areas.Admin.ApiControllers
{
    [ApiAdminTokenRequire]
    public class AdminUserWalletController : ApiBaseController
    {
        [HttpGet]
        public JsonResult GetTotalBalanceUser()
        {
            try
            {
                AdminUserWalletService adminUserWalletService = new AdminUserWalletService();
                return Success(adminUserWalletService.GetTotalBalanceUser(), "Lấy dữ liệu thành công!");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}
