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
    public class AdminSystemIncomeYearReportController : ApiBaseController
    {
        [HttpGet]
        public JsonResult GetTotalRevenue()
        {
            try
            {
                AdminSystemIncomeYearReportService adminSystemIncomeYearReportService = new AdminSystemIncomeYearReportService();
                return Success(adminSystemIncomeYearReportService.GetTotalRevenue(), "Lấy dữ liệu thành công!");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}
