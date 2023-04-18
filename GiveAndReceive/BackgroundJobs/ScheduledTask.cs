using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using GiveAndReceive.Models;
using GiveAndReceive.Services;
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
                IJobDetail nextQuestForUserTask = JobBuilder.Create<NextQuestForUserTask>()
                            .WithIdentity("NextQuestForUserJob", "NextQuestForUserGroup")
                            .Build();

                ITrigger triggerNextQuestForUserTask = TriggerBuilder.Create()
                    .WithIdentity("NextQuestForUserTrigger", "NextQuestForUserGroup")
                    .StartNow()
                    .WithCronSchedule("* 0/3 * * * ? *")
                    .Build();
                //await scheduler.AddJob(userConnectTask, true);

                IList<ITrigger> listTriggerNextQuestForUserTask = new List<ITrigger>();
                listTriggerNextQuestForUserTask.Add(triggerNextQuestForUserTask);
                IReadOnlyCollection<ITrigger> listOfTriggersUserConnect = new ReadOnlyCollection<ITrigger>(listTriggerNextQuestForUserTask);

                await scheduler.ScheduleJob(nextQuestForUserTask, listOfTriggersUserConnect, true);
                #endregion



                await scheduler.Start();
            }
            catch (Exception ex) { }

        }
    }
}