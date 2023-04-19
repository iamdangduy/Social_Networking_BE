using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GiveAndReceive.Models;
using GiveAndReceive.Services;
using GiveAndReceive.Providers;
namespace GiveAndReceive.ApiControllers
{
    public class UserQuestController : ApiBaseController
    {

        [HttpGet]
        public JsonResult GetListQuest()
        {
            try
            {
                User user = UserProvider.GetUserFromRequestHeader(Request);

                QueueGiveQuestService queueGiveQuestService = new QueueGiveQuestService();
                QueueGiveService queueGiveService = new QueueGiveService();
                QueueGive queueGive = queueGiveService.GetUserQueueGiveInDuty(user.UserId);
                if (queueGive == null) queueGive = new QueueGive();
                List<object> listQueueGiveQuest = queueGiveQuestService.GetListCurrentUserQuestView(queueGive.QueueGiveId);

                return Success(new
                {
                    queueGive,
                    listQueueGiveQuest
                });
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }


        }


        [HttpGet]
        public JsonResult GetListGive(int page, string status) {
            try {
                User user = UserProvider.GetUserFromRequestHeader(Request);
                QueueGiveQuestService queueGiveQuestService = new QueueGiveQuestService();
                return Success(queueGiveQuestService.GetListUserGiveQuest(user.UserId,status,page));
            }
            catch (Exception ex) {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetQuestDetail(string questId)
        {
            try
            {
                User user = UserProvider.GetUserFromRequestHeader(Request);
                QueueGiveQuestService queueGiveQuestService = new QueueGiveQuestService();
                QueueReceiveService queueReceiveService = new QueueReceiveService();
                QueueGiveService queueGiveService = new QueueGiveService();
                QueueGiveQuest queueGiveQuest = queueGiveQuestService.GetQueueGiveQuest(questId);
                UserService userService = new UserService();
                QueueGive queueGive = queueGiveService.GetQueueGive(queueGiveQuest.QueueGiveId);
                if(queueGive == null) throw new Exception("Không tìm thấy thông tin giao dịch");

                QueueReceive queueReceive = queueReceiveService.GetQueueReceive(queueGiveQuest.QueueReceiveId);
                if (queueReceive.UserId != user.UserId) throw new Exception("Không tìm thấy thông tin giao dịch");

                User userGive = userService.GetUserById(queueGive.UserId);
                return Success(new {
                    queueGiveQuest.AmountGive,
                    queueGiveQuest.Code,
                    queueGiveQuest.ExpireTime,
                    queueGiveQuest.QueueGiveQuestId,
                    queueGiveQuest.Status,
                    queueGiveQuest.TransactionImage,
                    userGive.Name,
                    userGive.Account,
                    userGive.Avatar,
                    userGive.Phone,
                    userGive.Email
                });
            }
            catch (Exception ex) {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult ConfirmSent(QueueGiveQuest model)
        {

            try
            {
                if (string.IsNullOrEmpty(model.TransactionImage)) throw new Exception("Bạn cần gửi kèm ảnh chụp giao dịch thành công để người nhận có thể xác nhận");

                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        QueueGiveQuestService queueGiveQuestService = new QueueGiveQuestService(connection);
                        QueueReceiveService queueReceiveService = new QueueReceiveService(connection);
                        QueueGiveService queueGiveService = new QueueGiveService(connection);
                        UserService userService = new UserService(connection);

                        QueueGiveQuest queueGiveQuest = queueGiveQuestService.GetQueueGiveQuest(model.QueueGiveQuestId, transaction);
                        if (queueGiveQuest == null) throw new Exception("Không tìm thấy thông tin nhiệm vụ");

                        QueueReceive queueReceive = queueReceiveService.GetQueueReceive(queueGiveQuest.QueueReceiveId, transaction);
                        if (queueReceive == null) throw new Exception("Không tìm thấy thông tin nhiệm vụ");

                        QueueGive queueGive = queueGiveService.GetQueueGive(queueGiveQuest.QueueGiveId, transaction);
                        if (queueGive == null) throw new Exception("Không tìm thấy thông tin nhiệm vụ");

                        User userReceive = userService.GetUserById(queueReceive.UserId, transaction);
                        User userGive = userService.GetUserById(queueGive.UserId, transaction);

                        //luu anh giao dich
                        string fileName = Guid.NewGuid().ToString() + ".png";
                        string pathSave = System.Web.HttpContext.Current.Server.MapPath("~" + String.Format(Constant.PATH.QUEUE_GIVE_QUEST_TRANSACTION_IMAGE_URL, userGive.UserId));
                        HelperProvider.Base64ToImage(model.TransactionImage, pathSave + fileName);

                        queueGiveQuest.Status = QueueGiveQuest.EnumStatus.SENT;
                        queueGiveQuest.TransactionImage = String.Format(Constant.PATH.QUEUE_GIVE_QUEST_TRANSACTION_IMAGE_URL, userGive.UserId) + fileName;
                        queueGiveQuestService.UpdateQueueGiveQuest(queueGiveQuest, transaction);

                        //kiem tra do hoan thanh nhiem vu cua nguoi cho
                        List<QueueGiveQuest> listQuest = queueGiveQuestService.GetListCurrentUserQuest(queueGive.QueueGiveId, transaction);
                        int totalQuestPending = listQuest.Count(x => x.Status == QueueGiveQuest.EnumStatus.PENDING);
                        int totalQuestCancel = listQuest.Count(x => x.Status == QueueGiveQuest.EnumStatus.CANCEL);

                        if (totalQuestCancel + totalQuestPending <= 0)
                        {
                            queueGive.Status = QueueGive.EnumStatus.WAITING_CONFIRM;
                            queueGiveService.UpdateQueueGive(queueGive,transaction);
                        }
                        
                        if (!string.IsNullOrEmpty(userReceive.Email))
                            SMSProvider.SendOTPViaEmail(userReceive.Email, "", "[KING SMART] THÔNG BÁO XÁC NHẬN", userGive.Name + " đã chuyển khoản cho bạn. Hãy kiểm tra tài khoản và sau đó đăng nhập hệ thống để xác nhận giao dịch này.");

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
        public JsonResult ConfirmReceive(string questId)
        {
            try
            {
                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        QueueGiveQuestService queueGiveQuestService = new QueueGiveQuestService(connection);
                        QueueReceiveService queueReceiveService = new QueueReceiveService(connection);
                        QueueGiveService queueGiveService = new QueueGiveService(connection);
                        UserService userService = new UserService(connection);

                        QueueGiveQuest queueGiveQuest = queueGiveQuestService.GetQueueGiveQuest(questId, transaction);
                        if (queueGiveQuest == null) throw new Exception("Không tìm thấy thông tin nhiệm vụ");
                        
                        QueueReceive queueReceive = queueReceiveService.GetQueueReceive(queueGiveQuest.QueueReceiveId, transaction);
                        if (queueReceive == null) throw new Exception("Không tìm thấy thông tin nhiệm vụ");

                        QueueGive queueGive = queueGiveService.GetQueueGive(queueGiveQuest.QueueGiveId, transaction);
                        if (queueGive == null) throw new Exception("Không tìm thấy thông tin nhiệm vụ");

                        if(queueGiveQuest.Status != QueueGiveQuest.EnumStatus.SENT) throw new Exception("Trạng thái nhiệm vụ không hợp lệ để xác nhận đã nhận");

                        User userReceive = userService.GetUserById(queueReceive.UserId, transaction);
                        User userGive = userService.GetUserById(queueGive.UserId, transaction);

                        queueGiveQuest.Status = QueueGiveQuest.EnumStatus.DONE;
                        queueGiveQuestService.UpdateQueueGiveQuest(queueGiveQuest, transaction);

                        UserPropertiesService userPropertiesService = new UserPropertiesService(connection) ;
                        userPropertiesService.UpdateTotalAmountGive(userGive.UserId, queueGiveQuest.AmountGive, transaction);
                        userPropertiesService.UpdateTotalAmountReceive(userReceive.UserId, queueGiveQuest.AmountGive, transaction);

                        /*
                        List<QueueGiveQuest> listQuest = queueGiveQuestService.GetListCurrentUserQuest(queueGiveQuest.QueueGiveId, transaction);
                        int totalDone = listQuest.Count(x => x.Status == QueueGiveQuest.EnumStatus.DONE);
                        if (totalDone == listQuest.Count) { 
                            
                        }
                        */

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
        public JsonResult GetListUserQueueGive()
        {
            try
            {
                User user = UserProvider.GetUserFromRequestHeader(Request);

                QueueGiveQuestService queueGiveQuestService = new QueueGiveQuestService();

                return Success(queueGiveQuestService.GetListUserQueueGive(user.UserId));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetListUserQueueReceive()
        {
            try
            {
                User user = UserProvider.GetUserFromRequestHeader(Request);

                QueueGiveQuestService queueGiveQuestService = new QueueGiveQuestService();

                return Success(queueGiveQuestService.GetListUserQueueReceive(user.UserId));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}
