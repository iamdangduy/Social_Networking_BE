using GiveAndReceive.Filters;
using GiveAndReceive.Models;
using GiveAndReceive.Providers;
using GiveAndReceive.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace GiveAndReceive.Areas.Admin.ApiControllers
{
    public class AdminSystemAboutController : ApiAdminBaseController
    {
        // GET: Admin/AdminSystemAbout
        [HttpGet]
        [ApiAdminTokenRequire]
        public JsonResult GetSystemAbout()
        {
            try
            {
                UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                if (userAdmin == null) return Unauthorized();
                SystemAboutService systemAboutService = new SystemAboutService();
                return Success(systemAboutService.GetSystemAbout(), "Lấy dữ liệu thành công!!");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [ApiAdminTokenRequire]
        public JsonResult UpdateSystemAbout(SystemAbout model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                        if (userAdmin == null) return Unauthorized();
                        SystemAboutService systemAboutService = new SystemAboutService(connect);
                        systemAboutService.UpdateSystemAbout(model, transaction);
                        transaction.Commit();
                        return Success(null, "Sửa thành công!!");
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
