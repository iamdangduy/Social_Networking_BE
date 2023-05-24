using GiveAndReceive.ApiControllers;
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

namespace GiveAndReceive.Areas.Admin.ApiControllers
{
    public class AdminSystemController : ApiAdminBaseController
    {
        [AllowAnonymous]
        [HttpPost]
        public JsonResult LoginPost(UserLoginPost model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Email)) return Error("Tài khoản không được để trống.");
                if (string.IsNullOrEmpty(model.Password)) return Error("Mật khẩu không được để trống.");

                UserAdminService adminService = new UserAdminService();

                UserAdmin admin = adminService.GetUserAdminByAccount(model.Email);
                if (admin == null) return Error("Sai tài khoản hoặc mật khẩu.");

                string password = SecurityProvider.EncodePassword(admin.UserAdminId, model.Password);
                if (admin.Password != password) return Error("Sai tài khoản hoặc mật khẩu.");

                string token = SecurityProvider.CreateToken(admin.UserAdminId, admin.Password, admin.Account);
                adminService.UpdateUserAdminToken(admin.UserAdminId, token);
                return Success(token);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        [ApiAdminTokenRequire]
        public JsonResult LogOut()
        {
            try
            {
                UserAdminService userAdminService = new UserAdminService();
                UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                if (userAdmin == null) return Unauthorized();
                userAdminService.UpdateUserAdminToken(userAdmin.UserAdminId, "");
                return Success();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}
