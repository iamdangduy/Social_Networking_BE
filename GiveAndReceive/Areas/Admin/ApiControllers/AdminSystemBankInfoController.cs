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

namespace GiveAndReceive.Areas.Admin.ApiControllers
{
    public class AdminSystemBankInfoController : ApiAdminBaseController
    {
        [HttpPost]
        [ApiAdminTokenRequire]
        public JsonResult AdminCreateSystemBankInfo(SystemBankInfo model)
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
                        AdminSystemBankInfoService adminSystemBankInfoService = new AdminSystemBankInfoService(connect);

                        if (model.BankName == null || model.BankOwnerName == null || model.BankNumber == null) throw new Exception("Vui lòng nhập đủ thông tin");
                        model.SystemBankInfoId = Guid.NewGuid().ToString();
                        if (model.IsDefault == true)
                        {
                            SystemBankInfo systemBankInfoDefault = adminSystemBankInfoService.GetSystemBankInfoIsDefault(transaction);
                            if (systemBankInfoDefault != null) adminSystemBankInfoService.SetSystemBankInfoIsDefaultFalse(systemBankInfoDefault.SystemBankInfoId, transaction);
                        }
                        adminSystemBankInfoService.InsertSystemBankInfo(model, transaction);
                        transaction.Commit();
                        return Success();
                    }
                }
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [ApiAdminTokenRequire]
        public JsonResult AdminUpdateSystemBankInfo(SystemBankInfo model)
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
                        AdminSystemBankInfoService adminSystemBankInfoService = new AdminSystemBankInfoService(connect);
                        SystemBankInfo systemBankInfo = adminSystemBankInfoService.GetSystemBankInfoById(model.SystemBankInfoId, transaction);
                        if (systemBankInfo == null) throw new Exception("Không tìm thấy thông tin");
                        if (model.BankName == null || model.BankOwnerName == null || model.BankNumber == null) throw new Exception("Vui lòng nhập đủ thông tin");
                        if (model.IsDefault == true)
                        {
                            SystemBankInfo systemBankInfoDefault = adminSystemBankInfoService.GetSystemBankInfoIsDefault(transaction);
                            if (systemBankInfoDefault != null) adminSystemBankInfoService.SetSystemBankInfoIsDefaultFalse(systemBankInfoDefault.SystemBankInfoId, transaction);
                        }
                        adminSystemBankInfoService.UpdateSystemBankInfo(model, transaction);
                        transaction.Commit();
                        return Success();
                    }
                }
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        [ApiAdminTokenRequire]
        public JsonResult AdminDeleteSystemBankInfo(string SystemBankInfoId)
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
                        AdminSystemBankInfoService adminSystemBankInfoService = new AdminSystemBankInfoService(connect);

                        SystemBankInfo systemBankInfo = adminSystemBankInfoService.GetSystemBankInfoById(SystemBankInfoId, transaction);
                        if (systemBankInfo == null) throw new Exception("Không tìm thấy thông tin");

                        adminSystemBankInfoService.DeleteSystemBankInfo(systemBankInfo.SystemBankInfoId, transaction);
                        transaction.Commit();
                        return Success();
                    }
                }
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        [ApiAdminTokenRequire]
        public JsonResult GetListAllSystemBankInfo()
        {
            try
            {
                UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                if (userAdmin == null) return Unauthorized();
                AdminSystemBankInfoService adminSystemBankInfoService = new AdminSystemBankInfoService();

                List<SystemBankInfo> lsSystemBankInfo = adminSystemBankInfoService.GetListAllSystemBankInfo();
                return Success(lsSystemBankInfo);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
        [HttpGet]
        [ApiAdminTokenRequire]
        public JsonResult GetSystemBankInfoById(string SystemBankInfoId)
        {
            try
            {
                UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                if (userAdmin == null) return Unauthorized();
                AdminSystemBankInfoService adminSystemBankInfoService = new AdminSystemBankInfoService();

                SystemBankInfo systemBankInfo = adminSystemBankInfoService.GetSystemBankInfoById(SystemBankInfoId);
                if (systemBankInfo == null) throw new Exception("Không tìm thấy hạng");
                return Success(systemBankInfo);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}
