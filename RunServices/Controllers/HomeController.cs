using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using RunServices.Models;
using RunServices.Exeption;
using System.Globalization;

namespace RunServices.Controllers
{
    public class HomeController : Controller
    {
        [MyExpentions]
        // GET: Home
        public async Task<ActionResult> Index()
        {
            GlobalVariables.UserIP = Request.UserHostAddress;
            GlobalVariables.HostName = System.Net.Dns.GetHostEntry(Request.UserHostAddress).HostName;

            ServiceProcess services = new ServiceProcess();
            List<ConfigService> obj = services.ServicesNameStatus();
            return View(obj);
        }

        [HttpPost]
        [MyExpentions]
        public async Task<ActionResult> Restart(string serviceName)
        {
            ServiceProcess services = new ServiceProcess();
            await services.RestartService(serviceName, 10000, "Manual");
            return RedirectToAction("Index");
        }

        [HttpPost]
        [MyExpentions]
        public async Task<ActionResult> StartService(string serviceName)
        {
            ServiceProcess services = new ServiceProcess();
            await services.StartService(serviceName, "Manual");
            return RedirectToAction("Index");
        }

        [HttpPost]
        [MyExpentions]
        public async Task<ActionResult> Stop(string serviceName)
        {
            ServiceProcess services = new ServiceProcess();
            await services.StopService(serviceName, "Manual");
            return RedirectToAction("Index");
        }
    }
}