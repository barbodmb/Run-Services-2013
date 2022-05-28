using Quartz;
using Quartz.Impl;
using RunServices.Job;
using RunServices.Models;
using System.Web.Mvc;
using System.Web.Routing;

namespace RunServices
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            MyScheduler.Start();
        }
    }
}