using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RunServices.Job
{
    public class MyScheduler
    {
        public static void Start()
        {
            var scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();
            var job = JobBuilder.Create<ExcuteRunService>().Build();
            var trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule(
                    builder =>
                        builder.WithIntervalInMinutes(5)
                            .OnEveryDay()
                            .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0)))
            .Build();
            scheduler.ScheduleJob(job, trigger);
        }
    }
}