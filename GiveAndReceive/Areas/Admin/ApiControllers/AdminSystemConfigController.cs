using GiveAndReceive.ApiControllers;
using GiveAndReceive.Areas.Admin.Services;
using GiveAndReceive.Filters;
using GiveAndReceive.Models;
using GiveAndReceive.Providers;
using GiveAndReceive.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using static GiveAndReceive.Models.JsonResult;

namespace GiveAndReceive.Areas.Admin.ApiControllers
{
    public class AdminSystemConfigController : ApiAdminBaseController
    {
        [HttpGet]
        [ApiAdminTokenRequire]
        public JsonResult GetListConfig()
        {
            try
            {
                UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                if (userAdmin == null) return Unauthorized();

                AdminSystemConfigService adminSystemConfig = new AdminSystemConfigService();
                return Success(adminSystemConfig.GetListConfig(), "Lấy dữ liệu thành công!!");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [ApiAdminTokenRequire]
        public JsonResult UpdateConfig(SystemConfig model)
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
                        AdminSystemConfigService adminSystemConfig = new AdminSystemConfigService(connect);
                        SystemConfig systemConfig = adminSystemConfig.GetSystemConfigById(model.SystemConfigId, transaction);

                        if (systemConfig.ValueType == SystemConfig.EnumValueType.STRING || systemConfig.ValueType == SystemConfig.EnumValueType.NUMBER || systemConfig.ValueType == SystemConfig.EnumValueType.BOOLEAN)
                            systemConfig.Value = model.Value;

                        if (systemConfig.ValueType == SystemConfig.EnumValueType.IMAGE)
                        {
                            //xóa file cũ
                            if (!HelperProvider.DeleteFile(systemConfig.Value)) return Error(JsonResult.Message.ERROR_SYSTEM);
                            //tạo file mới
                            string filename = Guid.NewGuid().ToString() + ".jpg";
                            var path = System.Web.HttpContext.Current.Server.MapPath(Constant.SYSTEM_BANK_QR_IMAGE_PATH + filename);
                            HelperProvider.Base64ToImage(model.Value, path);
                            systemConfig.Value = Constant.SYSTEM_BANK_QR_IMAGE_URL + filename;
                        }

                        adminSystemConfig.UpdateConfig(systemConfig, transaction);
                        transaction.Commit();
                        return Success(null, "Sửa thành công!");
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