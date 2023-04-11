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

namespace GiveAndReceive.Areas.Admin.ApiControllers
{
    public class AdminUserWithdrawOrderController : ApiBaseController
    {
        [HttpGet]
        [ApiAdminTokenRequire]
        public JsonResult GetListUserWithdrawOrder(string keyword, int page)
        {
            try
            {
                AdminUserWithdrawOrderService adminUserWithdrawOrderService = new AdminUserWithdrawOrderService();
                return Success(adminUserWithdrawOrderService.GetListUserWithdrawOrder(page, keyword));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        [ApiAdminTokenRequire]
        public JsonResult ProcessUserWithdrawOrder(string userWithdrawOrderId)
        {
            try
            {
                using(var connect = BaseService.Connect())
                {
                    connect.Open();
                    using(var transaction = connect.BeginTransaction())
                    {
                        AdminUserWithdrawOrderService adminUserWithdrawOrderService = new AdminUserWithdrawOrderService(connect);
                        UserWithdrawOrder userWithdrawOrder = adminUserWithdrawOrderService.GetUserWithdrawOrderById(userWithdrawOrderId, transaction);
                        if (userWithdrawOrder == null) return Error();

                        if(userWithdrawOrder.Status != UserWithdrawOrder.EnumStatus.PENDING) return Error("Giao dịch này hiện tại không thể xử lý. Vui lòng thử lại sau.");

                        userWithdrawOrder.Status = UserWithdrawOrder.EnumStatus.PROCESSING;
                        adminUserWithdrawOrderService.UpdateUserWithdrawOrderStatus(userWithdrawOrder, transaction);

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

        [HttpGet]
        [ApiAdminTokenRequire]
        public JsonResult DoneUserWithdrawOrder(string userWithdrawOrderId)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        AdminUserWithdrawOrderService adminUserWithdrawOrderService = new AdminUserWithdrawOrderService(connect);
                        UserWithdrawOrder userWithdrawOrder = adminUserWithdrawOrderService.GetUserWithdrawOrderById(userWithdrawOrderId, transaction);
                        if (userWithdrawOrder == null) return Error();

                        if (userWithdrawOrder.Status != UserWithdrawOrder.EnumStatus.PROCESSING) return Error("Giao dịch này hiện tại không thể xử lý. Vui lòng thử lại sau.");

                        userWithdrawOrder.Status = UserWithdrawOrder.EnumStatus.DONE;
                        adminUserWithdrawOrderService.UpdateUserWithdrawOrderStatus(userWithdrawOrder, transaction);

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

        [HttpGet]
        [ApiAdminTokenRequire]
        public JsonResult CancelUserWithdrawOrder(string userWithdrawOrderId)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        
                        AdminUserWalletService adminUserWalletService = new AdminUserWalletService(connect);
                        AdminUserTransactionService adminUserTransactionService = new AdminUserTransactionService(connect);
                        AdminUserWithdrawOrderService adminUserWithdrawOrderService = new AdminUserWithdrawOrderService(connect);
                        UserWithdrawOrder userWithdrawOrder = adminUserWithdrawOrderService.GetUserWithdrawOrderById(userWithdrawOrderId, transaction);
                        if (userWithdrawOrder == null) return Error();

                        // Cập nhật trạng thái
                        userWithdrawOrder.Status = UserWithdrawOrder.EnumStatus.SYSTEM_DECLINE;
                        adminUserWithdrawOrderService.UpdateUserWithdrawOrderStatus(userWithdrawOrder, transaction);
                        DateTime now = DateTime.Now;
                        UserWithdrawOrderStatus userWithdrawOrderStatus = new UserWithdrawOrderStatus();
                        userWithdrawOrderStatus.UserWithdrawOrderStatusId = Guid.NewGuid().ToString();
                        userWithdrawOrderStatus.UserWithdrawOrderId = userWithdrawOrderId;
                        userWithdrawOrderStatus.Status = userWithdrawOrder.Status;
                        userWithdrawOrderStatus.CreateTime = HelperProvider.GetSeconds(now);
                        adminUserWithdrawOrderService.InsertUserWithdrawOrderStatus(userWithdrawOrderStatus, transaction);

                        // Cập nhật lại tiền trong ví người dùng
                        adminUserWalletService.UpdateBalanceByUserId(userWithdrawOrder.UserId, userWithdrawOrder.Amount, transaction);

                        UserTransaction userTransaction = new UserTransaction();
                        userTransaction.UserTransactionId = Guid.NewGuid().ToString();
                        userTransaction.UserId = userWithdrawOrder.UserId;
                        userTransaction.Amount = userWithdrawOrder.Amount;
                        userTransaction.Note = "Hệ thống hủy lệnh rút " + userWithdrawOrder.Amount.ToString() + " VND.";
                        userTransaction.CreateTime = HelperProvider.GetSeconds(now);

                        adminUserTransactionService.InsertUserTransaction(userTransaction, transaction);

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