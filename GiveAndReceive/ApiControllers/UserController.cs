﻿using GiveAndReceive.Filters;
using GiveAndReceive.Models;
using GiveAndReceive.Providers;
using GiveAndReceive.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GiveAndReceive.ApiControllers
{
    public class UserController : ApiBaseController
    {
        [HttpPost]
        public JsonResult ChangePassword(UserRequest model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        string token = Request.Headers.Authorization.ToString();

                        UserService userService = new UserService(connect);
                        User user = userService.GetUserByToken(token, transaction);
                        if (user == null) return Unauthorized();

                        string password = SecurityProvider.EncodePassword(user.UserId, model.Password);
                        if (!user.Password.Equals(password)) return Error("Mật khẩu cũ không đúng");
                        else
                        {
                            model.NewPassword = SecurityProvider.EncodePassword(user.UserId, model.NewPassword);
                            //userService.ChangePassword(user.UserId, model.NewPassword, transaction);

                            transaction.Commit();
                            return Success();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult UpdateInfoUser(User model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        string token = Request.Headers.Authorization.ToString();

                        UserService userService = new UserService(connect);
                        User user = userService.GetUserByToken(token, transaction);
                        if (user == null) return Unauthorized();

                        user.Name = model.Name.Trim();
                        if (!string.IsNullOrEmpty(model.Avatar))
                        {
                            string filename = Guid.NewGuid().ToString() + ".jpg";
                            /*var path = System.Web.HttpContext.Current.Server.MapPath(Constant.AVATAR_USER_PATH + filename);
                            HelperProvider.Base64ToImage(model.Avatar, path);
                            if (!HelperProvider.DeleteFile(user.Avatar)) return Error();
                            user.Avatar = Constant.AVATAR_USER_URL + filename;*/
                        }

                        if (!string.IsNullOrEmpty(model.Account))
                        {
                            user.Account = model.Account.Trim();
                            userService.CheckAccountExist(user.Account, transaction);
                        }

                        if (!string.IsNullOrEmpty(model.Email))
                        {
                            user.Email = model.Email.Trim();
                            userService.CheckEmailExist(user.Email, transaction);
                        }

                        user.Phone = model.Phone.Trim();

                        userService.UpdateUser(user, transaction);

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
        [AllowAnonymous]
        public JsonResult GetVerifyCode(string phone)
        {

            if (string.IsNullOrEmpty(phone)) return Error("Số điện thoại không được để trống.");

            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        UserService userService = new UserService(connect);
                        CodeConfirmService codeConfirmService = new CodeConfirmService(connect);
                        User user = userService.GetUserByPhone(phone, transaction);
                        if (user != null) return Error("Số điện thoại đã tồn tại trên hệ thống.");

                        int codeConfirmCheck = codeConfirmService.CountCodeConfirmOfEOPIn24Hours(phone, transaction);
                        if (codeConfirmCheck >= 3) return Error("Bạn đã dùng hết 3 lượt lấy OTP bằng điện thoại. Vui lòng thử lại sau 24 giờ.");

                        Random rnd = new Random();
                        int code = rnd.Next(100000, 999999);
                        CodeConfirm codeConfirm = new CodeConfirm();
                        codeConfirm.CodeConfirmId = Guid.NewGuid().ToString();
                        codeConfirm.Phone = phone;
                        codeConfirm.Code = code.ToString();
                        codeConfirmService.InsertCodeConfirm(codeConfirm, transaction);
                        if (!SMSProvider.SendOTPViaPhone(phone, codeConfirm.Code)) return Error("Quá trình gửi gặp lỗi. Vui lòng thử lại sau");
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
        [AllowAnonymous]
        public JsonResult ConfirmCode(string phone, string code)
        {
            if (string.IsNullOrEmpty(code)) return Error("Mã xác nhận không được để trống!");
            if (string.IsNullOrEmpty(phone)) return Error("Số điện thoại không được để trống!");
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        UserService userService = new UserService(connect);
                        CodeConfirmService codeConfirmService = new CodeConfirmService(connect);
                        CodeConfirm codeConfirm = codeConfirmService.GetCodeConfirmByPhone(phone, transaction);
                        if (codeConfirm == null) return Error("Mã xác nhận không chính xác.");
                        if (!codeConfirm.Code.Equals(code)) return Error("Mã xác nhận không chính xác.");
                        if (codeConfirm.ExpiryTime < DateTime.Now) return Error("Mã xác nhận đã hết hạn.");
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
        [AllowAnonymous]
        public JsonResult Register(User userRequest)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {

                        UserService userService = new UserService(connect);
                        UserPropertiesService userPropertiesService = new UserPropertiesService(connect);
                        if (string.IsNullOrEmpty(userRequest.Name)) return Error("Họ và tên không được để trống.");
                        if (string.IsNullOrEmpty(userRequest.Password)) return Error("Mật khẩu không được để trống.");

                        User user = new User();
                        user.UserId = Guid.NewGuid().ToString();
                        user.Password = SecurityProvider.EncodePassword(user.UserId, userRequest.Password);
                        user.Name = userRequest.Name.Trim();
                        user.CreateTime = HelperProvider.GetSeconds();
                        if (!string.IsNullOrEmpty(userRequest.ParentCode))
                        {
                            User parent = userService.GetUserByShareCode(userRequest.ParentCode, transaction);
                            if (parent == null) throw new Exception("Mã giới thiệu không hợp lệ");
                            user.ParentCode = userRequest.ParentCode;
                            user.ShareCode = HelperProvider.MakeCode();
                        }
                        user.Phone = userRequest.Phone.Trim();
                        if (userService.CheckPhoneExist(user.Phone, transaction)) return Error("Số điện thoại đã tồn tại");
                        userService.InsertUser(user, transaction);

                        UserProperties userProperties = new UserProperties();
                        userProperties.UserId = user.UserId;
                        userProperties.RankId = 0;
                        userProperties.IdentificationApprove = false;
                        userProperties.TotalAmountGive = 0;
                        userProperties.TotalAmountReceive = 0;
                        userPropertiesService.CreateUserProperties(userProperties, transaction);

                        UserToken userToken = new UserToken();
                        userToken.UserTokenId = Guid.NewGuid().ToString();
                        userToken.UserId = user.UserId;
                        userToken.CreateTime = HelperProvider.GetSeconds();
                        userService.InsertUserToken(userToken, transaction);

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
        public JsonResult Login(UserLoginPost model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        if (string.IsNullOrEmpty(model.Account) || string.IsNullOrEmpty(model.Password)) return Error(JsonResult.Message.LOGIN_ACCOUNT_OR_PASSWORD_EMPTY);
                        UserService userService = new UserService(connect);

                        User userLogin = userService.GetUserByEmailOrPhoneOrAccount(model.Account, transaction);
                        if (userLogin == null) throw new Exception(JsonResult.Message.LOGIN_ACCOUNT_OR_PASSWORD_INCORRECT);

                        string password = SecurityProvider.EncodePassword(userLogin.UserId, model.Password);
                        if (!userLogin.Password.Equals(password)) return Error(JsonResult.Message.LOGIN_ACCOUNT_OR_PASSWORD_INCORRECT);

                        string deviceId = Guid.NewGuid().ToString().ToLower();
                        string token = SecurityProvider.CreateToken(userLogin.UserId, userLogin.Password, deviceId);

                        userService.UpdateUserToken(userLogin.UserId, token, transaction);
                        transaction.Commit();
                        return Success(new { token, deviceId });
                    }
                }
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        [ApiTokenRequire]
        public JsonResult Logout()
        {
            try
            {
                string token = Request.Headers.Authorization.ToString();
                UserService userService = new UserService();
                userService.RemoveUserToken(token);
                return Success();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetOTPCode(string phone)
        {

            if (string.IsNullOrEmpty(phone)) return Error("Số điện thoại không được để trống.");

            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        UserService userService = new UserService(connect);
                        CodeConfirmService codeConfirmService = new CodeConfirmService(connect);
                        User user = userService.GetUserByPhone(phone, transaction);
                        if (user == null) return Error("Số điện thoại không tồn tại trên hệ thống.");


                        int codeConfirmCheck = codeConfirmService.CountCodeConfirmOfEOPIn24Hours(phone, transaction);
                        if (codeConfirmCheck >= 3) return Error("Bạn đã dùng hết 3 lượt lấy OTP bằng điện thoại. Vui lòng thử lại sau 24 giờ.");

                        Random rnd = new Random();
                        int code = rnd.Next(100000, 999999);
                        CodeConfirm codeConfirm = new CodeConfirm();
                        codeConfirm.CodeConfirmId = Guid.NewGuid().ToString();
                        codeConfirm.Phone = phone;
                        codeConfirm.Code = code.ToString();
                        codeConfirmService.InsertCodeConfirm(codeConfirm, transaction);
                        if (!SMSProvider.SendOTPViaPhone(user.Phone, codeConfirm.Code)) return Error("Quá trình gửi gặp lỗi. Vui lòng thử lại sau");
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
        [AllowAnonymous]
        public JsonResult CheckOTPCode(string phone, string code)
        {
            if (string.IsNullOrEmpty(code)) return Error("Mã xác nhận không được để trống.");
            if (string.IsNullOrEmpty(phone)) return Error("Số điện thoại không được để trống.");
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        CodeConfirmService codeConfirmService = new CodeConfirmService(connect);
                        UserService userService = new UserService(connect);

                        User user = userService.GetUserByPhone(phone, transaction);
                        if (user == null) return Error("Số điện thoại không tồn tại trên hệ thống.");

                        CodeConfirm codeConfirm = codeConfirmService.GetCodeConfirmByPhone(phone, transaction);
                        if (codeConfirm == null) return Error("Mã xác nhận không tồn tại.");
                        if (!codeConfirm.Code.Equals(code)) return Error("Mã xác nhận không chính xác.");
                        if (codeConfirm.ExpiryTime < DateTime.Now) return Error("Mã xác nhận đã hết hạn.");
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
        [AllowAnonymous]
        public JsonResult ForgotPassword(string phone, string code, string newPassword)
        {
            if (string.IsNullOrEmpty(code)) return Error("Mã xác nhận không được để trống.");
            if (string.IsNullOrEmpty(phone)) return Error("Số điện thoại không được để trống.");
            if (string.IsNullOrEmpty(newPassword)) return Error("Mật khẩu không được để trống.");
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        UserService userService = new UserService(connect);
                        CodeConfirmService codeConfirmService = new CodeConfirmService(connect);

                        if (string.IsNullOrEmpty(phone)) return Error("Yêu cầu nhập số điện thoại cần gửi mã!");
                        User user = userService.GetUserByPhone(phone, transaction);
                        if (user == null) return Error("Số điện thoại không tồn tại trên hệ thống.");

                        CodeConfirm codeConfirm = codeConfirmService.GetCodeConfirmByPhone(phone, transaction);
                        if (codeConfirm == null) return Error("Mã xác nhận không tồn tại.");
                        if (!codeConfirm.Code.Equals(code)) return Error("Mã xác nhận không chính xác.");
                        if (codeConfirm.ExpiryTime < DateTime.Now) return Error("Mã xác nhận đã hết hạn.");

                        user.Password = SecurityProvider.EncodePassword(user.UserId, newPassword);
                        userService.ChangePassword(user.UserId, user.Password, transaction);
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
