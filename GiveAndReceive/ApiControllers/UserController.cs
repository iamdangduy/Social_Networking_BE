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
                        if(!string.IsNullOrEmpty(model.Avatar))
                        {
                            string filename = Guid.NewGuid().ToString() + ".jpg";
                            /*var path = System.Web.HttpContext.Current.Server.MapPath(Constant.AVATAR_USER_PATH + filename);
                            HelperProvider.Base64ToImage(model.Avatar, path);
                            if (!HelperProvider.DeleteFile(user.Avatar)) return Error();
                            user.Avatar = Constant.AVATAR_USER_URL + filename;*/
                        }

                        if(!string.IsNullOrEmpty(model.Account))
                        {
                            user.Account = model.Account.Trim();
                            userService.CheckAccountExist(user.Account, transaction);
                        }

                        if(!string.IsNullOrEmpty (model.Email))
                        {
                            user.Email = model.Email.Trim();
                            userService.CheckEmailExist(user.Email, transaction);
                        }

                        user.Phone = model.Phone.Trim();

                        userService.UpdateUser(user,transaction);

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
