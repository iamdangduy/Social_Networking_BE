using GiveAndReceive.Filters;
using GiveAndReceive.Models;
using GiveAndReceive.Providers;
using GiveAndReceive.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using static GiveAndReceive.Models.JsonResult;

namespace GiveAndReceive.ApiControllers
{
    public class UserBankInfoController : ApiBaseController
    {
        [HttpGet]
        [ApiAdminTokenRequire]
        public JsonResult GetListUserBankInfo()
        {
            try
            {
                string token = Request.Headers.Authorization.ToString();
                UserService userService = new UserService();
                User user = userService.GetUserByToken(token);
                if (user == null) return Unauthorized();

                UserBankInfoService userBankInfoService = new UserBankInfoService();
                return Success(userBankInfoService.GetListUserBankInfo(user.UserId), "Lấy dữ liệu thành công!");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [ApiAdminTokenRequire]
        public JsonResult InsertUserBankInfo(UserBankInfo model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        string token = Request.Headers.Authorization.ToString();
                        UserService userService = new UserService();
                        User user = userService.GetUserByToken(token);
                        if (user == null) return Unauthorized();

                        UserBankInfo userBankInfo = new UserBankInfo();
                        userBankInfo.UserBankInfoId = Guid.NewGuid().ToString();
                        userBankInfo.UserId = user.UserId;
                        userBankInfo.BankName = model.BankName;
                        userBankInfo.BankOwnerName = model.BankOwnerName;
                        userBankInfo.BankNumber = model.BankNumber;
                        userBankInfo.IsDefault = true;

                        if (!Directory.Exists(HttpContext.Current.Server.MapPath("~" + String.Format(Constant.SYSTEM_BANK_QR_IMAGE_PATH))))
                        {
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~" + String.Format(Constant.SYSTEM_BANK_QR_IMAGE_PATH)));
                        }
                        //tạo file mới
                        if (model.QRImage == null) return Error();
                        string filename = Guid.NewGuid().ToString() + ".jpg";
                        var path = System.Web.HttpContext.Current.Server.MapPath(Constant.SYSTEM_BANK_QR_IMAGE_PATH + filename);
                        HelperProvider.Base64ToImage(model.QRImage, path);
                        userBankInfo.QRImage = Constant.SYSTEM_BANK_QR_IMAGE_URL + filename;

                        UserBankInfoService userBankInfoService = new UserBankInfoService(connect);
                        userBankInfoService.InsertUserBankInfo(userBankInfo, transaction);

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
        public JsonResult UpdateUserBankInfo(UserBankInfo model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        string token = Request.Headers.Authorization.ToString();
                        UserService userService = new UserService();
                        User user = userService.GetUserByToken(token);
                        if (user == null) return Unauthorized();

                        UserBankInfo userBankInfo = new UserBankInfo();
                        userBankInfo.UserBankInfoId = model.UserBankInfoId;
                        userBankInfo.BankName = model.BankName;
                        userBankInfo.BankOwnerName = model.BankOwnerName;
                        userBankInfo.BankNumber = model.BankNumber;

                        if (string.IsNullOrEmpty(model.QRImage)) return Error();
                        //tạo file mới
                        if (model.QRImage == null) return Error();
                        string filename = Guid.NewGuid().ToString() + ".jpg";
                        var path = System.Web.HttpContext.Current.Server.MapPath(Constant.SYSTEM_BANK_QR_IMAGE_PATH + filename);
                        HelperProvider.Base64ToImage(model.QRImage, path);
                        userBankInfo.QRImage = Constant.SYSTEM_BANK_QR_IMAGE_URL + filename;

                        UserBankInfoService userBankInfoService = new UserBankInfoService(connect);
                        userBankInfoService.UpdateUserBankInfo(userBankInfo, transaction);

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
        public JsonResult DeleteUserBankInfo(string UserBankInfoId)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        string token = Request.Headers.Authorization.ToString();
                        UserService userService = new UserService();
                        User user = userService.GetUserByToken(token);
                        if (user == null) return Unauthorized();

                        UserBankInfoService userBankInfoService = new UserBankInfoService(connect);
                        userBankInfoService.DeleteUserBankInfo(UserBankInfoId, transaction);

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
    }
}
