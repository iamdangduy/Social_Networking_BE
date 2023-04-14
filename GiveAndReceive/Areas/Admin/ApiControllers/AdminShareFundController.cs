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
    public class AdminShareFundController : ApiAdminBaseController
    {
        [HttpGet]
        public JsonResult GetBalanceShareFund()
        {
            try
            {
                AdminShareFundService adminShareFundService = new AdminShareFundService();
                return Success(adminShareFundService.GetBalanceShareFund(), "Lấy dữ liệu thành công!");

            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}
