using Quartz;
using Quartz.Core;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RunServices.Job
{
    public class UnitySchedulerFactory : StdSchedulerFactory
    {
        private readonly UnityJobFactory unityJobFactory;

        public UnitySchedulerFactory(UnityJobFactory unityJobFactory)
        {
            this.unityJobFactory = unityJobFactory;
        }

        protected override IScheduler Instantiate(QuartzSchedulerResources rsrcs, QuartzScheduler qs)
        {
            qs.JobFactory = this.unityJobFactory;
            return base.Instantiate(rsrcs, qs);
        }
    }
}