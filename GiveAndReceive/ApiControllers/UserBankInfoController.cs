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

namespace GiveAndReceive.ApiControllers
{
    public class UserBankInfoController : ApiBaseController
    {
        [HttpGet]
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
        [HttpGet]
        public JsonResult GetListUserBankInfoByUserId(string userId)
        {
            try
            {
                string token = Request.Headers.Authorization.ToString();
                UserService userService = new UserService();
                User user = userService.GetUserByToken(token);
                if (user == null) return Unauthorized();

                User user1 = userService.GetUserById(userId);
                if (user1 == null) throw new Exception("Không tìm thấy người dùng này");

                UserBankInfoService userBankInfoService = new UserBankInfoService();
                return Success(userBankInfoService.GetListUserBankInfo(user1.UserId), "Lấy dữ liệu thành công!");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
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
                        userBankInfo.IsDefault = model.IsDefault;

                        if (!Directory.Exists(HttpContext.Current.Server.MapPath("~" + String.Format(Constant.PATH.SYSTEM_BANK_QR_IMAGE_PATH))))
                        {
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~" + String.Format(Constant.PATH.SYSTEM_BANK_QR_IMAGE_PATH)));
                        }
                        //tạo file mới
                        if (model.QRImage == null) return Error();
                        string filename = Guid.NewGuid().ToString() + ".jpg";
                        var path = System.Web.HttpContext.Current.Server.MapPath(Constant.PATH.SYSTEM_BANK_QR_IMAGE_PATH + filename);
                        HelperProvider.Base64ToImage(model.QRImage, path);
                        userBankInfo.QRImage = Constant.PATH.SYSTEM_BANK_QR_IMAGE_URL + filename;

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
                        UserBankInfoService userBankInfoService = new UserBankInfoService(connect);

                        UserBankInfo userBankInfo = new UserBankInfo();
                        userBankInfo.UserBankInfoId = model.UserBankInfoId;
                        userBankInfo.BankName = model.BankName;
                        userBankInfo.BankOwnerName = model.BankOwnerName;
                        userBankInfo.BankNumber = model.BankNumber;
                        userBankInfo.IsDefault = model.IsDefault;
                        var UserBankInforModel = userBankInfoService.GetUserBankInfoById(model.UserBankInfoId, transaction);

                        if (!string.IsNullOrEmpty(model.QRImage))
                        {
                            //xóa file cũ
                            if (!HelperProvider.DeleteFile(UserBankInforModel.QRImage)) return Error(JsonResult.Message.ERROR_SYSTEM);
                            //tạo file mới
                            if (model.QRImage == null) return Error();
                            string filename = Guid.NewGuid().ToString() + ".jpg";
                            var path = HttpContext.Current.Server.MapPath(Constant.PATH.SYSTEM_BANK_QR_IMAGE_PATH + filename);
                            HelperProvider.Base64ToImage(model.QRImage, path);
                            userBankInfo.QRImage = Constant.PATH.SYSTEM_BANK_QR_IMAGE_URL + filename;
                        }

                        if (string.IsNullOrEmpty(model.QRImage)) userBankInfo.QRImage = UserBankInforModel.QRImage;
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
