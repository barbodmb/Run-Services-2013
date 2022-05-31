using Quartz;
using RunServices.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace RunServices.Job
{
    public class ExcuteRunService : IJob
    {
        private readonly ServiceProcess _serviceProcess = new ServiceProcess();
        public void Execute(IJobExecutionContext context)
        {
            _serviceProcess.RunAutoServices(_serviceProcess.GetStopServices(_serviceProcess.ServicesNameStatus()));
        }
    }
}