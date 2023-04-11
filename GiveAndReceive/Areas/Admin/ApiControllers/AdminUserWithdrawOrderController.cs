using GiveAndReceive.ApiControllers;
using GiveAndReceive.Areas.Admin.Services;
using GiveAndReceive.Models;
using GiveAndReceive.Providers;
using GiveAndReceive.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace GiveAndReceive.Areas.Admin.ApiControllers
{
    public class AdminUserWithdrawOrderController : ApiBaseController
    {
        [HttpGet]
        public JsonResult GetListUserWithdrawOrder(string keyword, int page)
        {
            try
            {
                UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                if (userAdmin == null) return Unauthorized();

                AdminUserWithdrawOrderService adminUserWithdrawOrderService = new AdminUserWithdrawOrderService();
                return Success(adminUserWithdrawOrderService.GetListUserWithdrawOrder(page, keyword));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        public JsonResult ProcessUserWithdrawOrder(string userWithdrawOrderId)
        {
            try
            {
                using(var connect = BaseService.Connect())
                {
                    connect.Open();
                    using(var transaction = connect.BeginTransaction())
                    {
                        UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                        if (userAdmin == null) return Unauthorized();

                        AdminUserWithdrawOrderService adminUserWithdrawOrderService = new AdminUserWithdrawOrderService(connect);
                        UserWithdrawOrder userWithdrawOrder = adminUserWithdrawOrderService.GetUserWithdrawOrderById(userWithdrawOrderId);
                        if (userWithdrawOrder == null) return Error();

                        if(userWithdrawOrder.Status != UserWithdrawOrder.EnumStatus.PENDING) return Error("Giao dịch này hiện tại không thể xử lý. Vui lòng thử lại sau.");

                        userWithdrawOrder.Status = UserWithdrawOrder.EnumStatus.PROCESSING;
                        adminUserWithdrawOrderService.UpdateUserWithdrawOrderStatus(userWithdrawOrder);

                        UserWithdrawOrderStatus userWithdrawOrderStatus = new UserWithdrawOrderStatus();
                        userWithdrawOrderStatus.UserWithdrawOrderStatusId = Guid.NewGuid().ToString();
                        userWithdrawOrderStatus.UserWithdrawOrderId = userWithdrawOrderId;
                        userWithdrawOrderStatus.Status = userWithdrawOrder.Status;
                        userWithdrawOrderStatus.CreateTime = HelperProvider.GetSeconds();
                        adminUserWithdrawOrderService.InsertUserWithdrawOrderStatus(userWithdrawOrderStatus, transaction);

                        transaction.Commit();
                        return Success();
                    }
                }
            }
            catch(Exception ex)
            {
                return Error(ex.Message);
            }
        }

        public JsonResult CancelUserWithdrawOrder(string userWithdrawOrderId)
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

                        AdminUserWithdrawOrderService adminUserWithdrawOrderService = new AdminUserWithdrawOrderService(connect);
                        UserWithdrawOrder userWithdrawOrder = adminUserWithdrawOrderService.GetUserWithdrawOrderById(userWithdrawOrderId);
                        if (userWithdrawOrder == null) return Error();

                        //if (userWithdrawOrder.Status != UserWithdrawOrder.EnumStatus.PENDING) return Error("Giao dịch này hiện tại không thể xử lý. Vui lòng thử lại sau.");

                        userWithdrawOrder.Status = UserWithdrawOrder.EnumStatus.SYSTEM_DECLINE;
                        adminUserWithdrawOrderService.UpdateUserWithdrawOrderStatus(userWithdrawOrder);

                        UserWithdrawOrderStatus userWithdrawOrderStatus = new UserWithdrawOrderStatus();
                        userWithdrawOrderStatus.UserWithdrawOrderStatusId = Guid.NewGuid().ToString();
                        userWithdrawOrderStatus.UserWithdrawOrderId = userWithdrawOrderId;
                        userWithdrawOrderStatus.Status = userWithdrawOrder.Status;
                        userWithdrawOrderStatus.CreateTime = HelperProvider.GetSeconds();
                        adminUserWithdrawOrderService.InsertUserWithdrawOrderStatus(userWithdrawOrderStatus, transaction);



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