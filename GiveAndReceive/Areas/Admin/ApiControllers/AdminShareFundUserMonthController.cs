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
    public class AdminShareFundUserMonthController : ApiAdminBaseController
    {
        [HttpGet]
        [ApiAdminTokenRequire]
        public JsonResult GetListShareFundUserMonth(string shareFundUserMonthId, int page)
        {
            try
            {
                AdminShareFundUserMonthService adminShareFundUserMonthService = new AdminShareFundUserMonthService();
                return Success(adminShareFundUserMonthService.GetListShareFundUserMonthByMonthYear(shareFundUserMonthId,page));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}