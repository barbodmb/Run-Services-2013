using Quartz;
using RunServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RunServices.Job
{
    public class ExcuteRunService : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            ServiceProcess c = new ServiceProcess();
            c.StartService("MSSQLSERVER");
        }
    }
}