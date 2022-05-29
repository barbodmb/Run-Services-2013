using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using RunServices.Models;

namespace RunServices.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public async Task<ActionResult> Index()
        {
            ServiceProcess test = new ServiceProcess();
            List<ConfigService> m = test.ServicesNameStatus();
            return View(m);
        }
        [HttpPost]
        public async Task<ActionResult> Restart(string serviceName)
        {
            ServiceProcess test = new ServiceProcess();
            await test.RestartService(serviceName, 10000);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> StartService(string serviceName)
        {
            ServiceProcess test = new ServiceProcess();
            await test.StartService(serviceName);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Stop(string serviceName)
        {
            ServiceProcess test = new ServiceProcess();
            await test.StopService(serviceName);
            return RedirectToAction("Index");
        }
    }
}