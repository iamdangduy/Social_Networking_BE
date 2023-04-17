using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using GiveAndReceive.Models;
using GiveAndReceive.Services;
using GiveAndReceive.Providers;
using System.Data;

namespace GiveAndReceive.BackgroundJobs
{
    public class NextQuestForUserTask : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                long now = HelperProvider.GetSeconds();
                DateTime dateNow = DateTime.Now;

                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {


                        QueueGiveService queueGiveService = new QueueGiveService(connection);
                        QueueReceiveService queueReceiveService = new QueueReceiveService(connection);
                        QueueGiveQuestService queryGiveQuestService = new QueueGiveQuestService(connection);
                        UserService userService = new UserService(connection);

                        //lấy người đang làm nhiệm vụ hiện tại
                        QueueGive queueGive = queueGiveService.GetQueueGiveInduty(transaction);
                        if (queueGive == null)
                        {//nếu ko có người đang làm nhiệm vụ hiện tại, lấy đầu hàng đợi
                            CreateQuestForUser(now, dateNow, connection, transaction);
                            transaction.Commit();
                            return;
                        }

                        User user = userService.GetUserById(queueGive.UserId, transaction);
                        if (user == null) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        
                        List<QueueGiveQuest> listQueueGiveQuest = queryGiveQuestService.GetListCurrentUserQuest(user.UserId, transaction);

                        int questPending = listQueueGiveQuest.Count(x => x.Status == QueueGiveQuest.EnumStatus.PENDING);

                        #region nếu danh sách nhiệm vụ chưa làm vẫn còn
                        if (questPending > 0)
                        {
                            List<QueueGiveQuest> listQueueGiveQuestPending = listQueueGiveQuest.Where(x => x.Status == QueueGiveQuest.EnumStatus.PENDING).ToList();
                            // kiểm tra nhiệm vụ còn hạn không
                            if (listQueueGiveQuestPending.Count(x => x.ExpireTime < now) > 0)//nếu nhiệm vụ còn hạn, return 
                            {
                                return;
                            }
                            else
                            { //nếu nhiệm vụ hết hạn

                                //thoát người dùng khỏi hàng đợi
                                queueGive.Status = QueueGive.EnumStatus.CANCEL;
                                queueGiveService.UpdateQueueGive(queueGive, transaction);
                                queryGiveQuestService.CancelQueueGiveQuest(queueGive.QueueGiveId, transaction);

                                //chuyển tiếp nhiệm vụ cho người khác
                                CreateQuestForUser(now,dateNow,connection,transaction);
                            }
                        }
                        #endregion

                        transaction.Commit();
                    }
                }

            }
            catch (Exception ex)
            {

            }

        }

        public void CreateQuestForUser(long now, DateTime dateNow, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            QueueGiveService queueGiveService = new QueueGiveService(connection);
            QueueReceiveService queueReceiveService = new QueueReceiveService(connection);
            QueueGiveQuestService queueGiveQuestService = new QueueGiveQuestService(connection);

            QueueGive firstUserInQueue = queueGiveService.GetFirstPendingInQueue(transaction);
            if (firstUserInQueue == null) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
            firstUserInQueue.Status = QueueGive.EnumStatus.IN_DUTY;
            queueGiveService.UpdateQueueGive(firstUserInQueue, transaction);

            List<QueueReceive> listQueueReceive = queueReceiveService.GetListAvaiableQueueReceive(transaction);
            if (listQueueReceive.Count <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

            QueueReceive randomBot = queueReceiveService.GetRandomBot(transaction);

            SystemConfigService systemConfigService = new SystemConfigService(connection);
            SystemConfig limitGive = systemConfigService.GetSystemConfig(SystemConfig.EnumSystemConfigId.LIMIT_GIVE);
            if (limitGive == null) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
            long amountNeedGiveLeft = long.Parse(limitGive.Value);

            SystemConfig limitReceive = systemConfigService.GetSystemConfig(SystemConfig.EnumSystemConfigId.LIMIT_RECEIVE);
            if (limitReceive == null) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
            long limitReceiveTotal = long.Parse(limitReceive.Value);

            foreach (var item in listQueueReceive)
            {
                if (amountNeedGiveLeft <= 0) break;

                QueueGiveQuest queueGiveQuest = new QueueGiveQuest();
                queueGiveQuest.CreateTime = now;
                queueGiveQuest.ExpireTime = HelperProvider.GetSeconds(dateNow.AddHours(24));
                queueGiveQuest.QueueGiveId = firstUserInQueue.QueueGiveId;
                queueGiveQuest.QueueGiveQuestId = Guid.NewGuid().ToString();
                queueGiveQuest.Status = QueueGiveQuest.EnumStatus.PENDING;

                if (item.TotalExpectReceiveAmount == 0)
                {
                    queueGiveQuest.QueueReceiveId = randomBot.QueueReceiveId;
                    queueGiveQuest.AmountGive = 300000;

                    amountNeedGiveLeft -= queueGiveQuest.AmountGive;
                }
                else
                {


                    queueGiveQuest.QueueReceiveId = item.QueueReceiveId;

                    if (amountNeedGiveLeft > item.TotalExpectReceiveAmount)
                    {
                        queueGiveQuest.AmountGive = item.TotalExpectReceiveAmount;
                        amountNeedGiveLeft -= item.TotalExpectReceiveAmount;
                    }
                    else
                    {
                        queueGiveQuest.AmountGive = amountNeedGiveLeft;
                        amountNeedGiveLeft = 0;
                    }

                    if (queueGiveQuest.AmountGive + item.TotalExpectReceiveAmount > limitReceiveTotal) {
                        amountNeedGiveLeft = queueGiveQuest.AmountGive;
                        queueGiveQuest.AmountGive = limitReceiveTotal - item.TotalExpectReceiveAmount;
                        amountNeedGiveLeft = amountNeedGiveLeft - queueGiveQuest.AmountGive;
                    }


                    queueReceiveService.UpdateTotalExpectReceiveAmount(queueGiveQuest.QueueReceiveId, queueGiveQuest.AmountGive, transaction);
                }

                queueGiveQuestService.InsertQueueGiveQuest(queueGiveQuest, transaction);
            }

            Notification notification = new Notification();
            notification.CreateTime = now;
            notification.IsRead = false;
            notification.Link = "/";
            notification.Message = "<p>Hệ thống đã sắp xếp nhiệm vụ cho bạn. Hãy kiểm tra nhiệm vụ và hoàn thành sớm nhất có thể nhé</p>";
            notification.MessageShort = "Hệ thống đã sắp xếp nhiệm vụ cho bạn";
            notification.NotificationId = Guid.NewGuid().ToString();
            notification.UserId = firstUserInQueue.UserId;
            NotificationService notificationService = new NotificationService(connection);
            notificationService.CreateNotification(notification,transaction);

            UserService userService = new UserService(connection);
            User user = userService.GetUserById(firstUserInQueue.UserId, transaction);

            if (!string.IsNullOrEmpty(user.Email)) SMSProvider.SendOTPViaEmail(user.Email, "", "[KingSmart] Thông báo Nhiệm vụ", "Hệ thống đã sắp xếp nhiệm vụ cho bạn. Hãy đăng nhập vào hế thổng để kiểm tra nhiệm vụ và hoàn thành sớm nhất có thể nhé");

        }
    }
}