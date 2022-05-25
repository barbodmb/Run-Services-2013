using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunServices.Job
{
    public interface IQuartzScheduler
    {
        void Run();
        void Stop();
    }
}
