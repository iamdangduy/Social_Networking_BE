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
                        try
                        {
                            string token = Request.Headers.Authorization.ToString();

                            UserService userService = new UserService(connect);
                            User user = userService.GetUserByToken(token, transaction);
                            if (user == null) return Error("Người dùng không tồn tại");

                            string password = SecurityProvider.EncodePassword(user.UserId, model.Password);
                            if (!user.Password.Equals(password)) return Error("Mật khẩu cũ không đúng");
                            else
                            {
                                model.NewPassword = SecurityProvider.EncodePassword(user.UserId, model.NewPassword);
                                userService.ChangePassword(user.UserId, model.NewPassword, transaction);

                                transaction.Commit();
                                return Success();
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Error(ex.Message);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Error();
            }
        }

        [HttpPost]
        public JsonResult UpdateInfoUser(UpdateUser model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        string token = Request.Headers.Authorization.ToString();

                        CodeConfirmService codeConfirmService = new CodeConfirmService(connect);
                        UserService userService = new UserService(connect);
                        User user = userService.GetUserByToken(token, transaction);
                        if (user == null) return Unauthorized();

                        if (!string.IsNullOrEmpty(model.Avatar))
                        {
                            string filename = Guid.NewGuid().ToString() + ".jpg";
                            var path = System.Web.HttpContext.Current.Server.MapPath(Constant.PATH.AVATAR_USER_PATH + filename);
                            HelperProvider.Base64ToImage(model.Avatar, path);
                            if (!HelperProvider.DeleteFile(user.Avatar)) return Error();
                            user.Avatar = Constant.PATH.AVATAR_USER_URL + filename;
                        }

                        if (!string.IsNullOrEmpty(model.Account))
                        {
                            user.Account = model.Account.Trim();
                            userService.CheckAccountExist(user.Account, user.UserId, transaction);
                        }

                        if (!string.IsNullOrEmpty(model.Email))
                        {
                            userService.CheckEmailExist(model.Email, user.UserId, transaction);
                            if (model.Email != user.Email)
                            {
                                if (string.IsNullOrEmpty(model.EmailCode)) throw new Exception("Bạn chưa nhập mã xác thực email");
                                CodeConfirm emailCode = codeConfirmService.GetCodeConfirmByEmail(model.Email, transaction);
                                if (emailCode.Code != model.EmailCode) throw new Exception("Mã xác thực không chính xác");
                            }
                            user.Email = model.Email.Trim();
                        }
                        if (!string.IsNullOrEmpty(model.Phone))
                        {
                            userService.CheckUserPhoneExist(model.Phone, user.UserId, transaction);
                            if (model.Phone != user.Phone)
                            {
                                if (string.IsNullOrEmpty(model.PhoneCode)) throw new Exception("Bạn chưa nhập mã xác thực số điện thoại");
                                CodeConfirm phoneCode = codeConfirmService.GetCodeConfirmByPhone(model.Phone, transaction);
                                if (phoneCode.Code != model.PhoneCode) throw new Exception("Mã xác thực không chính xác");
                            }

                            user.Phone = model.Phone.Trim();
                        }

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
        public JsonResult GetVerifyCodeEmail(string email)
        {

            if (string.IsNullOrEmpty(email)) return Error("Email không được để trống.");

            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                       
                        CodeConfirmService codeConfirmService = new CodeConfirmService(connect);

                        Random rnd = new Random();
                        int code = rnd.Next(100000, 999999);
                        CodeConfirm codeConfirm = new CodeConfirm();
                        codeConfirm.CodeConfirmId = Guid.NewGuid().ToString();
                        codeConfirm.Email = email;
                        codeConfirm.Code = code.ToString();
                        codeConfirmService.InsertCodeConfirm(codeConfirm, transaction);

                        if (!SMSProvider.SendOTPViaEmail(email, codeConfirm.Code, "Mã xác nhận","" )) return Error("Quá trình gửi gặp lỗi. Vui lòng thử lại sau");
                        transaction.Commit();
                        return Success(codeConfirm.Code);
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
        public JsonResult ConfirmEmailCode(string email, string code)
        {
            if (string.IsNullOrEmpty(code)) return Error("Mã xác nhận không được để trống!");
            if (string.IsNullOrEmpty(email)) return Error("Email không được để trống!");
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        UserService userService = new UserService(connect);
                        CodeConfirmService codeConfirmService = new CodeConfirmService(connect);
                        CodeConfirm codeConfirm = codeConfirmService.GetCodeConfirmByEmail(email, transaction);
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
                        UserWalletService userWalletService = new UserWalletService(connect);
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
                        userProperties.Status = UserProperties.EnumStatus.CANCEL;
                        userPropertiesService.CreateUserProperties(userProperties, transaction);

                        UserToken userToken = new UserToken();
                        userToken.UserTokenId = Guid.NewGuid().ToString();
                        userToken.UserId = user.UserId;
                        userToken.CreateTime = HelperProvider.GetSeconds();
                        userService.InsertUserToken(userToken, transaction);

                        UserWallet userWallet = new UserWallet();
                        userWallet.UserId = user.UserId;
                        userWallet.Balance = 0;
                        userWallet.Pin = 0;
                        userWalletService.InsertUserWallet(userWallet, transaction);

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

                        DateTime now = DateTime.Now;
                        UserToken userToken = new UserToken();
                        userToken.CreateTime = HelperProvider.GetSeconds(now);
                        userToken.ExpireTime=HelperProvider.GetSeconds(now.AddDays(7));
                        userToken.Token = token;
                        userToken.UserId = userLogin.UserId;
                        userToken.UserTokenId = Guid.NewGuid().ToString();
                        userService.InsertUserToken(userToken, transaction);
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
                        if (codeConfirmCheck >= 3) return Error("Bạn đã dùng hết 5 lượt lấy OTP bằng điện thoại. Vui lòng thử lại sau 24 giờ.");

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

        [HttpGet]
        public JsonResult GetInforUser()
        {
            try
            {
                string token = Request.Headers.Authorization.ToString();
                UserService userService = new UserService();
                User user = userService.GetUserByToken(token);
                if (user == null) return Unauthorized();

                User userInfor = userService.GetUserInfo(user.UserId);
                return Success(userInfor);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetListConnectMember()
        {
            try
            {
                string token = Request.Headers.Authorization.ToString();
                UserService userService = new UserService();
                User user = userService.GetUserByToken(token);
                if (user == null) return Unauthorized();

                if (user.ShareCode == null) throw new Exception("Người dùng này không có mã giới thiệu");

                return Success(userService.GetListUserByShareCode(user.ShareCode));
            }
            catch(Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult ConnectMember(string code)
        {
            try
            {
                using(var connect = BaseService.Connect())
                {
                    connect.Open();
                    using(var transaction = connect.BeginTransaction())
                    {
                        string token = Request.Headers.Authorization.ToString();
                        UserService userService = new UserService(connect);
                        User user = userService.GetUserByToken(token, transaction);
                        if (user == null) return Unauthorized();

                        user.ShareCode = HelperProvider.MakeCode();
                        user.ParentCode = code;
                        User user1 = userService.GetUserByShareCode(user.ParentCode, transaction);
                        if (user1 == null) throw new Exception("Mã giới thiệu không đúng.");
                        user.Depth = user1.Depth + 1;

                        userService.UpdateUserCode(user, transaction);

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
