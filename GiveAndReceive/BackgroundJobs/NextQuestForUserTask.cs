using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using GiveAndReceive.Models;
using GiveAndReceive.Services;
using GiveAndReceive.Providers;

namespace GiveAndReceive.BackgroundJobs
{
    public class NextQuestForUserTask : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {

                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        //lấy người đang làm nhiệm vụ hiện tại

                        QueueGiveService queueGiveService = new QueueGiveService(connection);
                        QueueReceiveService queueReceiveService = new QueueReceiveService(connection);
                        QueryGiveQuestService queryGiveQuestService = new QueryGiveQuestService(connection);
                        UserService userService = new UserService(connection);

                        QueueGive queueGive = queueGiveService.GetQueueGiveInduty(transaction);
                        if (queueGive == null) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        User user = userService.GetUserById(queueGive.UserId, transaction);
                        if (user == null) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        long now = HelperProvider.GetSeconds();
                        DateTime dateNow = DateTime.Now;
                        List<QueueGiveQuest> listQueueGiveQuest = queryGiveQuestService.GetListCurrentUserQuest(user.UserId, transaction);

                        int questPending = listQueueGiveQuest.Count(x => x.Status == QueueGiveQuest.EnumStatus.PENDING);

                        #region nếu danh sách nhiệm vụ chưa làm vẫn còn
                        if (questPending > 0)
                        {
                            List<QueueGiveQuest> listQueueGiveQuestPending = listQueueGiveQuest.Where(x => x.Status == QueueGiveQuest.EnumStatus.PENDING).ToList();
                            // kiểm tra nhiệm vụ còn hạn không
                            if (listQueueGiveQuestPending.Count(x => x.ExpireTime < now) > 0)//nếu nhiệm vụ còn hạn, return 
                            {
                                throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                            }
                            else
                            { //nếu nhiệm vụ hết hạn

                                //thoát người dùng khỏi hàng đợi
                                queueGive.Status = QueueGive.EnumStatus.CANCEL;
                                queueGiveService.UpdateQueueGive(queueGive, transaction);
                                queryGiveQuestService.CancelQueueGiveQuest(queueGive.QueueGiveId, transaction);

                                //chuyển tiếp nhiệm vụ cho người khác
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

                                
                                foreach (var item in listQueueReceive) {
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
                                    else {
                                        queueGiveQuest.QueueReceiveId = item.QueueReceiveId;

                                        if (amountNeedGiveLeft > item.TotalExpectReceiveAmount)
                                        {
                                            queueGiveQuest.AmountGive = item.TotalExpectReceiveAmount;
                                            amountNeedGiveLeft -= item.TotalExpectReceiveAmount;
                                        }
                                        else {
                                            queueGiveQuest.AmountGive = amountNeedGiveLeft;
                                        }
                                        queueReceiveService.UpdateTotalExpectReceiveAmount(queueGiveQuest.QueueReceiveId, queueGiveQuest.AmountGive, transaction);
                                    }

                                    queryGiveQuestService.InsertQueueGiveQuest(queueGiveQuest, transaction);
                                }
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
    }
}