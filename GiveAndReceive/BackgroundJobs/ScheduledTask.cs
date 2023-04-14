using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace GiveAndReceive.BackgroundJobs
{
    public class ScheduledTask
    {
        public static async System.Threading.Tasks.Task StartAsync()
        {
            try
            {
                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();


                #region NextQuestForUser
                IJobDetail userConnectTask = JobBuilder.Create<NextQuestForUserTask>()
                            .WithIdentity("userConnectJob", "userConnectGroup")
                            .Build();

                ITrigger triggerFristTimeInFristDayUserConnectTask = TriggerBuilder.Create()
                    .WithIdentity("userConnectTrigger", "userConnectGroup")
                    .StartNow()
                    .WithCronSchedule("0 2 0 1 * ?")
                    .Build();

                ITrigger triggerFristDayUserConnectTask = TriggerBuilder.Create()
                    .WithIdentity("userConnectTrigger1", "userConnectGroup")
                    .StartNow()
                    .WithCronSchedule("0 2,12,22,32,42,52 1-23 1 * ?")
                    .Build();

                ITrigger triggerUserConnectTask = TriggerBuilder.Create()
                    .WithIdentity("userConnectTrigger2", "userConnectGroup")
                    .StartNow()
                    .WithCronSchedule("0 2,12,22,32,42,52 * 2-31 * ?")
                    .Build();
                //await scheduler.AddJob(userConnectTask, true);

                IList<ITrigger> listTriggersUserConnect = new List<ITrigger>();
                listTriggersUserConnect.Add(triggerFristTimeInFristDayUserConnectTask);
                listTriggersUserConnect.Add(triggerFristDayUserConnectTask);
                listTriggersUserConnect.Add(triggerUserConnectTask);
                IReadOnlyCollection<ITrigger> listOfTriggersUserConnect = new ReadOnlyCollection<ITrigger>(listTriggersUserConnect);

                await scheduler.ScheduleJob(userConnectTask, listOfTriggersUserConnect, true);
                #endregion

   

                await scheduler.Start();
            }
            catch (Exception ex) { }

        }
    }
}