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
    public class AdminQueueReceiveController : ApiBaseController
    {
        [HttpGet]
        [ApiAdminTokenRequire]
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
    }
}
