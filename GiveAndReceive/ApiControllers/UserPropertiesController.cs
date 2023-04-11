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
    public class UserPropertiesController : ApiBaseController
    {
        [HttpPost]
        public JsonResult CreateUserPropertiesForIdentity(UserProperties model)
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
                        UserPropertiesService userPropertiesService = new UserPropertiesService(connect);
                        UserProperties userProperties = new UserProperties();

                        if (!Directory.Exists(HttpContext.Current.Server.MapPath("~" + String.Format(Constant.IDENTITY_THUMBNAIL_PATH))))
                        {
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~" + String.Format(Constant.IDENTITY_THUMBNAIL_PATH)));
                        }

                        if (string.IsNullOrEmpty(model.CitizenIdentificationImageFront)) return Error();
                        //tạo file mới
                        if (model.CitizenIdentificationImageFront == null) return Error();
                        string filename = Guid.NewGuid().ToString() + ".jpg";
                        var path = System.Web.HttpContext.Current.Server.MapPath(Constant.IDENTITY_THUMBNAIL_PATH + filename);
                        HelperProvider.Base64ToImage(model.CitizenIdentificationImageFront, path);
                        userProperties.CitizenIdentificationImageFront = Constant.IDENTITY_THUMBNAIL_URL + filename;

                        if (string.IsNullOrEmpty(model.CitizenIdentificationImageBack)) return Error();
                        //tạo file mới
                        if (model.CitizenIdentificationImageBack == null) return Error();
                        string filename1 = Guid.NewGuid().ToString() + ".jpg";
                        var path1 = System.Web.HttpContext.Current.Server.MapPath(Constant.IDENTITY_THUMBNAIL_PATH + filename1);
                        HelperProvider.Base64ToImage(model.CitizenIdentificationImageBack, path1);
                        userProperties.CitizenIdentificationImageBack = Constant.IDENTITY_THUMBNAIL_URL + filename1;

                        if (string.IsNullOrEmpty(model.PhotoFace)) return Error();
                        //tạo file mới
                        if (model.PhotoFace == null) return Error();
                        string filename2 = Guid.NewGuid().ToString() + ".jpg";
                        var path2 = System.Web.HttpContext.Current.Server.MapPath(Constant.IDENTITY_THUMBNAIL_PATH + filename2);
                        HelperProvider.Base64ToImage(model.PhotoFace, path2);
                        userProperties.PhotoFace = Constant.IDENTITY_THUMBNAIL_URL + filename2;

                        userProperties.CitizenIdentificationName = model.CitizenIdentificationName;
                        userProperties.CitizenIdentificationNumber = model.CitizenIdentificationNumber;
                        userProperties.CitizenIdentificationAddress = model.CitizenIdentificationAddress;
                        userProperties.CitizenIdentificationDateOf = model.CitizenIdentificationDateOf;
                        userProperties.CitizenIdentificationPlaceOf = model.CitizenIdentificationPlaceOf;
                        userProperties.IdentificationApprove = UserProperties.EnumIdentificationApprove.SYSTEM_DECLINE;
                        userProperties.UserId = user.UserId;

                        userPropertiesService.CreateUserPropertiesForIdentity(userProperties, transaction);

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
        public JsonResult UpdateUserPropertiesForIdentity(UserProperties model)
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
                        UserPropertiesService userPropertiesService = new UserPropertiesService(connect);
                        var UserPropertiesModel = userPropertiesService.GetUserPropertiesByUserId(user.UserId, transaction);
                        UserProperties userProperties = new UserProperties();

                        if (!Directory.Exists(HttpContext.Current.Server.MapPath("~" + String.Format(Constant.IDENTITY_THUMBNAIL_PATH))))
                        {
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~" + String.Format(Constant.IDENTITY_THUMBNAIL_PATH)));
                        }


                        if (!string.IsNullOrEmpty(model.CitizenIdentificationImageFront))
                        {
                            //xóa file cũ
                            if (!HelperProvider.DeleteFile(UserPropertiesModel.CitizenIdentificationImageFront)) return Error(JsonResult.Message.ERROR_SYSTEM);
                            //tạo file mới
                            if (model.CitizenIdentificationImageFront == null) return Error();
                            string filename = Guid.NewGuid().ToString() + ".jpg";
                            var path = System.Web.HttpContext.Current.Server.MapPath(Constant.IDENTITY_THUMBNAIL_PATH + filename);
                            HelperProvider.Base64ToImage(model.CitizenIdentificationImageFront, path);
                            userProperties.CitizenIdentificationImageFront = Constant.IDENTITY_THUMBNAIL_URL + filename;
                        }
                        else
                        {
                            userProperties.CitizenIdentificationImageFront = UserPropertiesModel.CitizenIdentificationImageFront;
                        }

                        if (!string.IsNullOrEmpty(model.CitizenIdentificationImageBack))
                        {
                            //xóa file cũ
                            if (!HelperProvider.DeleteFile(UserPropertiesModel.CitizenIdentificationImageBack)) return Error(JsonResult.Message.ERROR_SYSTEM);
                            //tạo file mới
                            if (model.CitizenIdentificationImageBack == null) return Error();
                            string filename1 = Guid.NewGuid().ToString() + ".jpg";
                            var path1 = System.Web.HttpContext.Current.Server.MapPath(Constant.IDENTITY_THUMBNAIL_PATH + filename1);
                            HelperProvider.Base64ToImage(model.CitizenIdentificationImageBack, path1);
                            userProperties.CitizenIdentificationImageBack = Constant.IDENTITY_THUMBNAIL_URL + filename1;
                        }
                        else
                        {
                            userProperties.CitizenIdentificationImageBack = UserPropertiesModel.CitizenIdentificationImageBack;
                        }

                        if (!string.IsNullOrEmpty(model.PhotoFace))
                        {
                            //xóa file cũ
                            if (!HelperProvider.DeleteFile(UserPropertiesModel.PhotoFace)) return Error(JsonResult.Message.ERROR_SYSTEM);
                            //tạo file mới
                            if (model.PhotoFace == null) return Error();
                            string filename2 = Guid.NewGuid().ToString() + ".jpg";
                            var path2 = System.Web.HttpContext.Current.Server.MapPath(Constant.IDENTITY_THUMBNAIL_PATH + filename2);
                            HelperProvider.Base64ToImage(model.PhotoFace, path2);
                            userProperties.PhotoFace = Constant.IDENTITY_THUMBNAIL_URL + filename2;
                        }
                        else
                        {
                            userProperties.PhotoFace = UserPropertiesModel.PhotoFace;
                        }

                        userProperties.CitizenIdentificationName = model.CitizenIdentificationName;
                        userProperties.CitizenIdentificationNumber = model.CitizenIdentificationNumber;
                        userProperties.CitizenIdentificationAddress = model.CitizenIdentificationAddress;
                        userProperties.CitizenIdentificationDateOf = model.CitizenIdentificationDateOf;
                        userProperties.CitizenIdentificationPlaceOf = model.CitizenIdentificationPlaceOf;
                        userProperties.IdentificationApprove = UserProperties.EnumIdentificationApprove.SYSTEM_DECLINE;
                        userProperties.UserId = user.UserId;

                        userPropertiesService.UpdateUserPropertiesForIdentity(userProperties, transaction);
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
