using Microsoft.Practices.Unity;
using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace RunServices.Job
{
    public class UnityJobFactory : IJobFactory
    {
        private readonly IUnityContainer container;

        static UnityJobFactory()
        {
        }

        public UnityJobFactory(IUnityContainer container)
        {
            this.container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobDetail = bundle.JobDetail;
            var jobType = jobDetail.JobType;

            try
            {
                return this.container.Resolve(jobType) as IJob;
            }
            catch (Exception ex)
            {
                throw new SchedulerException(string.Format(
                    CultureInfo.InvariantCulture,
                    "Cannot instantiate class '{0}'", new object[] { jobDetail.JobType.FullName }), ex);
            }
        }

        public void ReturnJob(IJob job)
        {
            // Nothing here. Unity does not maintain a handle to container created instances.
        }
    }
}