using GiveAndReceive.ApiControllers;
using GiveAndReceive.Filters;
using GiveAndReceive.Models;
using GiveAndReceive.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GiveAndReceive.Areas.Admin.ApiControllers
{
    public class AdminUserPropertiesController : ApiAdminBaseController
    {
        [HttpGet]
        [ApiAdminTokenRequire]
        public JsonResult GetUserPropertiesByUserId(string UserId)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        UserPropertiesService userPropertiesService = new UserPropertiesService(connect);
                        return Success(userPropertiesService.GetUserPropertiesByUserId(UserId, transaction), "Lấy dữ liệu thành công!");
                    }
                }

            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}
