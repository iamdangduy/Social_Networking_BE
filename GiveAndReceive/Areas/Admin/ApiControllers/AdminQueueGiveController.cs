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
    public class AdminQueueGiveController : ApiAdminBaseController
    {
        [HttpGet]
        public JsonResult GetListQueueGive(int PageIndex = 1)
        {
            try
            {
                AdminQueueGiveService adminQueueGiveService = new AdminQueueGiveService();
                return Success(adminQueueGiveService.GetListQueueGive(PageIndex), "Lấy dữ liệu thành công!");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetTotalQueueGive()
        {
            try
            {
                AdminQueueGiveService adminQueueGiveService = new AdminQueueGiveService();
                return Success(adminQueueGiveService.GetTotalQueueGive(), "Lấy dữ liệu thành công!");

            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}
