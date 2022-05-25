using RunServices.Job;
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

            var unityContainer = Microsoft.Web.Infrastructure.Bootstrapper.Initialise();
            unityContainer.Resolve<IQuartzScheduler>().Run();
        }
    }
}