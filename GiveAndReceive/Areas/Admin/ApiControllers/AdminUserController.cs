using GiveAndReceive.ApiControllers;
using GiveAndReceive.Models;
using GiveAndReceive.Providers;
using GiveAndReceive.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GiveAndReceive.Areas.Admin.ApiControllers
{
    public class AdminUserController : ApiBaseController
    {
        [HttpGet]
        public JsonResult GetListUser(int PageIndex, string Keyword)
        {
			try
			{
                UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                if (userAdmin == null) return Unauthorized();

                AdminUserService adminUserService = new AdminUserService();
                return Success(adminUserService.GetListUser(PageIndex, Keyword), "Lấy dữ liệu thành công!");
            }
			catch (Exception ex)
			{
                return Error(ex.Message);
			}
        }
    }
}
