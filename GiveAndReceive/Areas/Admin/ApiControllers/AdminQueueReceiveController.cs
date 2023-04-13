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
    public class AdminQueueReceiveController : ApiAdminBaseController
    {
        [HttpGet]
        public JsonResult GetListQueueReceive(int PageIndex = 1, string Status = "")
        {
            try
            {
                AdminQueueReceiveService adminQueueReceiveService = new AdminQueueReceiveService();
                if (Status != QueueReceive.EnumStatus.PENDING) return Error();

                return Success(adminQueueReceiveService.GetListQueueReceive(PageIndex, QueueReceive.EnumStatus.PENDING), "Lấy dữ liệu thành công!");

            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetTotalQueueReceive()
        {
            try
            {
                AdminQueueReceiveService adminQueueReceiveService = new AdminQueueReceiveService();
                return Success(adminQueueReceiveService.GetTotalQueueReceive(), "Lấy dữ liệu thành công!");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}
