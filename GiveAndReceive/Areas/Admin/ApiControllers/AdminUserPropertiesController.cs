using GiveAndReceive.ApiControllers;
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
    public class AdminUserPropertiesController : ApiAdminBaseController
    {
        [HttpGet]
        [ApiAdminTokenRequire]
        public JsonResult GetUserPropertiesByUserId(string UserId)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        UserPropertiesService userPropertiesService = new UserPropertiesService(connect);
                        return Success(userPropertiesService.GetUserPropertiesByUserId(UserId, transaction), "Lấy dữ liệu thành công!");
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
        public JsonResult UpdateIdentityToSystemDecline(string UserId, string Reason)
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

                        AdminUserPropertiesService adminUserPropertiesService = new AdminUserPropertiesService(connect);
                        var UserProperties = adminUserPropertiesService.GetUserPropertiesByUserId(UserId, transaction);
                        if (UserProperties == null) return Error();
                        if (UserProperties.IdentificationApprove != UserProperties.EnumIdentificationApprove.SYSTEM_ACCEPT) return Error();

                        //tạo thông báo
                        Notification notification = new Notification
                        {
                            NotificationId = Guid.NewGuid().ToString(),
                            UserId = UserId,
                            Message = "CMND của bạn đã bị từ chối vì: " + Reason,
                            IsRead = false,
                            CreateTime = HelperProvider.GetSeconds(),
                        };
                        AdminNotificationService adminNotificationService = new AdminNotificationService(connect);
                        if (!adminNotificationService.InsertNotification(notification, transaction)) return Error();
                        adminUserPropertiesService.UpdateStatusUserIdentity(UserId, UserProperties.EnumIdentificationApprove.SYSTEM_DECLINE, UserProperties.EnumStatus.CANCEL,transaction);
                        //if (!) return Error();

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
        public JsonResult UpdateIdentityToSuccess(string UserId)
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

                        AdminUserPropertiesService adminUserPropertiesService = new AdminUserPropertiesService(connect);
                        var UserProperties = adminUserPropertiesService.GetUserPropertiesByUserId(UserId, transaction);
                        if (UserProperties == null) return Error();
                        if (UserProperties.IdentificationApprove != UserProperties.EnumIdentificationApprove.SYSTEM_DECLINE) return Error();
                        //if (!) return Error();
                        adminUserPropertiesService.UpdateStatusUserIdentity(UserId, UserProperties.EnumIdentificationApprove.SYSTEM_ACCEPT, transaction);
                        //tạo thông báo
                        Notification notification = new Notification
                        {
                            NotificationId = Guid.NewGuid().ToString(),
                            UserId = UserId,
                            Message = "Thông tin của bạn đã được xác thực.",
                            IsRead = false,
                            CreateTime = HelperProvider.GetSeconds(),
                        };
                        AdminNotificationService adminNotificationService = new AdminNotificationService(connect);
                        if (!adminNotificationService.InsertNotification(notification, transaction)) return Error();
                        adminUserPropertiesService.UpdateStatusUserIdentity(UserId, UserProperties.EnumIdentificationApprove.SYSTEM_ACCEPT, UserProperties.EnumStatus.DONE, transaction);

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
