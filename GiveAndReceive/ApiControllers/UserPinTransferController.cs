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
    public class UserPinTransferController : ApiBaseController
    {
        [HttpGet]
        [ApiTokenRequire]
        public JsonResult GetListPinTransferByUser()
        {
            try
            {
                string token = Request.Headers.Authorization.ToString();
                UserService userService = new UserService();
                User user = userService.GetUserByToken(token);
                if (user == null) return Unauthorized();

                UserPinTransferService userPinTransferService = new UserPinTransferService();
                return Success(userPinTransferService.GetListPinTransferByUser(user.UserId));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }


        [HttpGet]
        [ApiTokenRequire]
        public JsonResult ActivatePin()
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

                        QueueGiveService queueGiveService = new QueueGiveService(connect);
                        QueueReceiveService queueReceiveService = new QueueReceiveService(connect);
                        UserWalletService userWalletService = new UserWalletService(connect);
                        UserPinTransferService userPinTransferService = new UserPinTransferService(connect);

                        // Kiểm tra pin trong ví người dùng
                        UserWallet userWallet = userWalletService.GetUserWalletByUser(user.UserId, transaction);
                        if (userWallet.Pin <= 0) throw new Exception("Bạn không có pin để kích hoạt.");

                        QueueGive queueGive = queueGiveService.GetQueueGiveByUserId(user.UserId, transaction);
                        if(queueGive != null && (queueGive.Status == QueueGive.EnumStatus.PENDING || queueGive.Status == QueueGive.EnumStatus.IN_DUTY)) throw new Exception("Bạn đã đang ở trong phòng chờ cho, không thể kích hoạt pin");

                        QueueReceive queueReceive = queueReceiveService.GetQueueReceiveByUserId(user.UserId, transaction);
                        if(queueReceive != null && (queueReceive.Status == QueueReceive.EnumStatus.PENDING || queueReceive.Status == QueueReceive.EnumStatus.IN_DUTY)) throw new Exception("Bạn đã đang ở trong phòng nhận cho, không thể kích hoạt pin");

                        queueGive = new QueueGive();
                        queueGive.QueueGiveId = Guid.NewGuid().ToString();
                        queueGive.UserId = user.UserId;
                        queueGive.Status = QueueGive.EnumStatus.PENDING;
                        queueGive.CreateTime = HelperProvider.GetSeconds(DateTime.Now);
                        queueGiveService.InsertQueueGive(queueGive, transaction);

                        // Cập nhật pin trong ví người dùng
                        userWalletService.UpdatePinByUserId(user.UserId, -1, transaction);

                        UserPinTransfer userPinTransfer = new UserPinTransfer();
                        userPinTransfer.UserPinTransferId = Guid.NewGuid().ToString();
                        userPinTransfer.UserGiveId = user.UserId;
                        userPinTransfer.Pin = 1;
                        userPinTransfer.Status = UserPinTransfer.EnumStatus.DONE;
                        userPinTransfer.Message = "Bạn sử dụng để kích hoạt pin";
                        userPinTransfer.CreateTime = HelperProvider.GetSeconds();
                        userPinTransferService.InsertUserPinTransfer(userPinTransfer, transaction);

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