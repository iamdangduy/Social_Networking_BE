﻿using GiveAndReceive.ApiControllers;
using GiveAndReceive.Areas.Admin.Services;
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
using System.Web.Razor.Tokenizer.Symbols;

namespace GiveAndReceive.Areas.Admin.ApiControllers
{
    public class AdminUserController : ApiAdminBaseController
    {
        [HttpGet]
        public JsonResult GetListUser(int PageIndex = 1, string Keyword = "", string Status = "")
        {
            try
            {
                UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                if (userAdmin == null) return Unauthorized();

                AdminUserService adminUserService = new AdminUserService();
                return Success(adminUserService.GetListUser(PageIndex, Keyword, Status), "Lấy dữ liệu thành công!");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetUserByUserId(string UserId)
        {
            try
            {
                UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                if (userAdmin == null) return Unauthorized();

                AdminUserService adminUserService = new AdminUserService();
                return Success(adminUserService.GetUserByUserId(UserId), "Lấy dữ liệu thành công!");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetTotalUser()
        {
            try
            {
                AdminUserService adminUserService = new AdminUserService();
                return Success(adminUserService.GetTotalUser(), "Lấy dữ liệu thành công!");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}
