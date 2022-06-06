using Quartz;
using RunServices.Models;

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