using GiveAndReceive.Filters;
using GiveAndReceive.Models;
using GiveAndReceive.Providers;
using GiveAndReceive.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace GiveAndReceive.ApiControllers
{
    public class UserWithdrawOrderController : ApiBaseController
    {
        [HttpPost]
        [ApiTokenRequire]
        public JsonResult CreateUserWithdrawOrder(UserWithdrawOrder model)
        {
            try
            {
               using(var connect = BaseService.Connect())
                {
                    connect.Open();
                    using(var transaction =  connect.BeginTransaction())
                    {
                        string token = Request.Headers.Authorization.ToString();
                        UserService userService = new UserService(connect);
                        User user = userService.GetUserByToken(token, transaction);
                        if (user == null) return Unauthorized();

                        UserWithdrawOrderService userWithdrawOrderService = new UserWithdrawOrderService(connect);
                        UserTransactionService userTransactionService = new UserTransactionService(connect);

                        List<UserWithdrawOrder> withdrawOrdersPending = userWithdrawOrderService.GetListWithdrawStatusByUser(user.UserId, UserWithdrawOrder.EnumStatus.PENDING, transaction);
                        List<UserWithdrawOrder> withdrawOrdersProcessing = userWithdrawOrderService.GetListWithdrawStatusByUser(user.UserId, UserWithdrawOrder.EnumStatus.PROCESSING, transaction);
                        if (withdrawOrdersPending.Count > 0 || withdrawOrdersProcessing.Count > 0) throw new Exception("Vui lòng đợi hoàn thành lệnh rút trước đó.");

                        UserWalletService userWalletService = new UserWalletService(connect);
                        UserWallet userWallet = userWalletService.GetUserWalletByUser(user.UserId, transaction);
                        // Kiểm tra số tiền rút với số tiền trong ví người dùng
                        if (userWallet.Balance < model.Amount) throw new Exception("Số tiền không đủ để rút");

                        
                        // Tạo lệnh rút
                        UserWithdrawOrder userWithdrawOrder = new UserWithdrawOrder();
                        userWithdrawOrder.UserWithdrawOrderId = Guid.NewGuid().ToString();
                        userWithdrawOrder.UserId = user.UserId;
                        userWithdrawOrder.Code = HelperProvider.MakeCode();
                        userWithdrawOrder.Amount = model.Amount;
                        userWithdrawOrder.Status = UserWithdrawOrder.EnumStatus.PENDING;
                        DateTime now = DateTime.Now;
                        userWithdrawOrder.CreateTime = HelperProvider.GetSeconds(now);

                        UserWithdrawOrderStatus userWithdrawOrderStatus = new UserWithdrawOrderStatus();
                        userWithdrawOrderStatus.UserWithdrawOrderStatusId = Guid.NewGuid().ToString();
                        userWithdrawOrderStatus.UserWithdrawOrderId = userWithdrawOrder.UserWithdrawOrderId;
                        userWithdrawOrderStatus.Status = userWithdrawOrder.Status;
                        userWithdrawOrderStatus.CreateTime = HelperProvider.GetSeconds(now);

                        userWithdrawOrderService.InsertUserWithdrawOrder(userWithdrawOrder, transaction);
                        userWithdrawOrderService.InsertUserWithdrawOrderStatus(userWithdrawOrderStatus, transaction);

                        // Cập nhật ví người dùng
                        userWalletService.UpdateBalanceByUserId(user.UserId, -model.Amount, transaction);

                        UserTransaction userTransaction = new UserTransaction();
                        userTransaction.UserTransactionId = Guid.NewGuid().ToString();
                        userTransaction.UserId = user.UserId;
                        userTransaction.Amount = - model.Amount;
                        userTransaction.Note = "Người dùng đã tạo lệnh rút " + model.Amount + " VNĐ.";
                        userTransaction.CreateTime = HelperProvider.GetSeconds(now);
                        userTransactionService.InsertUserTransaction(userTransaction, transaction);

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
        [ApiTokenRequire]
        public JsonResult UserCancelWithdrawOrder(string userWithdrawId)
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

                        UserWalletService userWalletService = new UserWalletService(connect);
                        UserWithdrawOrderService userWithdrawOrderService = new UserWithdrawOrderService(connect);
                        UserTransactionService userTransactionService = new UserTransactionService(connect);

                        UserWithdrawOrder userWithdrawOrder = userWithdrawOrderService.GetUserWithdrawOrderById(userWithdrawId, transaction);
                        if (userWithdrawOrder == null) throw new Exception("Lệnh rút này không tồn tại.");

                        // Cập nhật trạng thái
                        userWithdrawOrder.Status = UserWithdrawOrder.EnumStatus.USER_CANCEL;
                        userWithdrawOrderService.UpdateUserWithdrawOrderStatus(userWithdrawOrder, transaction);

                        DateTime now = DateTime.Now;
                        UserWithdrawOrderStatus userWithdrawOrderStatus = new UserWithdrawOrderStatus();
                        userWithdrawOrderStatus.UserWithdrawOrderStatusId = Guid.NewGuid().ToString();
                        userWithdrawOrderStatus.UserWithdrawOrderId = userWithdrawOrder.UserWithdrawOrderId;
                        userWithdrawOrderStatus.Status = userWithdrawOrder.Status;
                        userWithdrawOrderStatus.CreateTime = HelperProvider.GetSeconds(now);
                        userWithdrawOrderService.InsertUserWithdrawOrderStatus(userWithdrawOrderStatus, transaction);

                        // Cập nhật ví người dùng
                        userWalletService.UpdateBalanceByUserId(user.UserId, userWithdrawOrder.Amount, transaction);

                        UserTransaction userTransaction = new UserTransaction();
                        userTransaction.UserTransactionId = Guid.NewGuid().ToString();
                        userTransaction.UserId = user.UserId;
                        userTransaction.Amount = userWithdrawOrder.Amount;
                        userTransaction.Note = "Người dùng đã hủy lệnh rút " + userWithdrawOrder.Amount + " VNĐ.";
                        userTransaction.CreateTime = HelperProvider.GetSeconds(now);
                        userTransactionService.InsertUserTransaction(userTransaction, transaction);

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