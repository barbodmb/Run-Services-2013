using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RunServices.Job
{
    public class QuartzScheduler : IQuartzScheduler
    {
        private readonly ISchedulerFactory SchedulerFactory;
        private readonly IScheduler Scheduler;

        public QuartzScheduler(ISchedulerFactory schedulerFactory, IScheduler scheduler)
        {
            this.SchedulerFactory = schedulerFactory;
            this.Scheduler = scheduler;
        }

        public void Run()
        {
            IJobDetail dailyUserMailJob = new JobDetailImpl("DailyUserMailJob", null, typeof(Scheduler.SchedulerJob));
            // fire every time I open App/EveryDay
            ITrigger dailyUserMailTrigger = new SimpleTriggerImpl("DailyUserMailTrigger", 10,
                                                     new TimeSpan(0, 0, 0, 20));

            this.Scheduler.ScheduleJob(dailyUserMailJob, dailyUserMailTrigger);

            this.Scheduler.Start();
        }

        public void Stop()
        {
            this.Scheduler.Shutdown(false);
        }
    }
}