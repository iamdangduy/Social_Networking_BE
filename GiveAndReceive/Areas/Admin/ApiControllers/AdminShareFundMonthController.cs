using GiveAndReceive.ApiControllers;
using GiveAndReceive.Areas.Admin.Services;
using GiveAndReceive.Filters;
using GiveAndReceive.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace GiveAndReceive.Areas.Admin.ApiControllers
{
    public class AdminShareFundMonthController : ApiAdminBaseController
    {
        [HttpGet]
        [ApiAdminTokenRequire]
        public JsonResult GetListShareFundMonth(int page)
        {
            try
            {
                AdminShareFundMonthService adminShareFundMonthService = new AdminShareFundMonthService();
                return Success(adminShareFundMonthService.GetListShareFundMonth(page));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        [ApiAdminTokenRequire]
        public JsonResult GetCurrentBalance()
        {
            try
            {
                var date = new DateTime();
                AdminShareFundMonthService adminShareFundMonthService = new AdminShareFundMonthService();
                return Success(adminShareFundMonthService.GetCurrentBalance(date.Month, date.Year), "Lấy dữ liệu thành công!");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}