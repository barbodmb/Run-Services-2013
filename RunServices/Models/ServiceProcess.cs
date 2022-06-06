using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace RunServices.Models
{
    public class ServiceProcess
    {
        public ServiceProcess()
        {
            ConfigService = new ConfigService();
        }
        public string ServiceName { get; set; }
        private ConfigService ConfigService { get; set; }

        public List<ConfigService> ServicesNameStatus()
        {
            ConfigService = ConfigurationManager.GetSection(Models.ConfigService.SectionName) as ConfigService;

            var servicesInfo = new List<ConfigService>();
            var sectionDetail = new List<SectionDetail>();
            ServiceController[] services = ServiceController.GetServices();
            try
            {
                foreach (ServiceController service in services)
                {
                    if (ConfigService != null)
                        foreach (SectionDetail item in ConfigService.Items)
                        {
                            if (service.ServiceName.ToUpper() == item.ServiceName.ToUpper())
                            {
                                sectionDetail.Add(new SectionDetail
                                {
                                    ServiceName = item.ServiceName,// + ":ServiceName",
                                    DisplayName = item.DisplayName,// + ":DisplayName",
                                    Interval = item.Interval,// + ":Interval",
                                    IntervalUnit = item.IntervalUnit,// + ":IntervalUnit",
                                    Url = item.Url, // + ":Url",
                                    RequestTimeOut = item.RequestTimeOut,//+ ":RequestTimeOut"
                                });

                                servicesInfo.Add(new ConfigService
                                {
                                    ServiceName = service.ServiceName,
                                    ServiceStatus = service.Status.ToString(),
                                    Items = new ItemsCollection(sectionDetail)
                                });
                            }
                        }
                } //end of IsServiceInstall
            }
            catch
            {
            }
            return servicesInfo;

        }//end of ServicesNameStatus

        public async Task StartService(string serviceName, string actionState)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                if (service.Status == ServiceControllerStatus.Stopped)
                {
                    //int millisec = Environment.TickCount;
                    //TimeSpan timeout = TimeSpan.FromSeconds(millisec);
                    await Task.Run(() =>
                    {
                        service.Start();
                    });
                    service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(120));
                    Logging(serviceName, "Start", actionState);
                }
            }
            catch(Exception ex)
            {
                //Console.WriteLine("Can not open Service...");
            }
        }//end of Start

        public async Task StopService(string serviceName, string actionState)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                if (service.Status == ServiceControllerStatus.Running)
                {
                    //var millisec = Environment.TickCount;
                    //TimeSpan timeout = TimeSpan.FromSeconds(millisec);
                    await Task.Run(() =>
                    {
                        service.Stop();
                    });                    
                    service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(120));
                    Logging(serviceName, "Stop", actionState);
                }
            }
            catch (Exception m)
            {
                //Console.WriteLine("Can not open Service...");
            }
        }//end of Stop

        public async Task RestartService(string serviceName, int timeoutMilliseconds, string actionState)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                int millisec1 = Environment.TickCount;
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                await StopService(serviceName, actionState);
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                int millisec2 = Environment.TickCount;
                timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));

                await StartService(serviceName, actionState);
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                Logging(serviceName, "Restart", actionState);
            }
            catch
            {
                // ...
            }
        }//end of Restart

        public List<ConfigService> GetStopServices(List<ConfigService> servicesName)
        {
            var servicesInfo = new List<ConfigService>();
            var sectionDetail = new List<SectionDetail>();
            try
            {
                if (servicesName != null)
                {
                    bool requestStatus = false;
                    foreach (var item in servicesName)
                    {
                        foreach (SectionDetail itemDetail in item.Items.SectionDetail)
                        {
                            if (itemDetail.IntervalUnit == "M")
                            {
                                if (Convert.ToInt32(itemDetail.Interval) / DateTime.Now.Minute == 0)
                                //&& (DateTime.Now.Second <= 10))
                                {
                                    requestStatus = SendRequest(itemDetail.Url, itemDetail.RequestTimeOut);
                                    if (!requestStatus || item.ServiceStatus == ServiceControllerStatus.Stopped.ToString())
                                    //|| item.ServiceStatus == ServiceControllerStatus.StopPending.ToString())
                                    {
                                        servicesInfo.Add(new ConfigService
                                        {
                                            ServiceName = item.ServiceName,
                                            ServiceStatus = item.ServiceStatus,
                                            Items = new ItemsCollection(sectionDetail = item.Items.SectionDetail),
                                            IsPending = !requestStatus
                                        });
                                    }
                                }
                            }

                            if (itemDetail.IntervalUnit == "H")
                            {
                                if (Convert.ToInt32(itemDetail.Interval) / DateTime.Now.Hour == 0)
                                //&& (DateTime.Now.Minute == 0))
                                {
                                    var RequestStatus = SendRequest(itemDetail.Url, itemDetail.RequestTimeOut);
                                    if (!RequestStatus || item.ServiceStatus == ServiceControllerStatus.Stopped.ToString())
                                    //|| item.ServiceStatus == ServiceControllerStatus.StopPending.ToString())
                                    {
                                        servicesInfo.Add(new ConfigService
                                        {
                                            ServiceName = item.ServiceName,
                                            ServiceStatus = item.ServiceStatus,
                                            Items = new ItemsCollection(sectionDetail = item.Items.SectionDetail),
                                            IsPending = !RequestStatus
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            return servicesInfo;
        }

        public async Task RunAutoServices(List<ConfigService> configServices)
        {
            try
            {
                foreach (var item in configServices)
                {
                    foreach (SectionDetail itemSectionDetail in item.Items.SectionDetail)
                    {
                        if (itemSectionDetail != null && itemSectionDetail.IntervalUnit != "")
                        {
                            if (item.IsPending && (item.ServiceStatus != ServiceControllerStatus.Stopped.ToString()))
                            //|| item.ServiceStatus != ServiceControllerStatus.StopPending.ToString()))
                            {
                                await RestartService(item.ServiceName, 1000, "Auto");
                            }
                            else
                            {
                                await StartService(item.ServiceName, "Auto");
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }//end of RunAutoServices

        public bool SendRequest(string path, string requestTimeOut)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(Convert.ToInt32(requestTimeOut));
                    var response = httpClient.GetAsync(path).Result;

                    if (response.StatusCode == HttpStatusCode.OK || path == "")
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        public void Logging(string serviceName, string action, string actionState)
        {
            using (StreamWriter sw = new StreamWriter(GlobalVariables.path, true))
            {
                sw.WriteLine(GlobalVariables.UserIP + "\t" + GlobalVariables.HostName + "\t" + actionState + "\t" + action + "\t" + serviceName
                    + "\t" + GetPersianDate(DateTime.Now) + "  " + DateTime.Now.ToString("HH:mm"));
            }
        }

        private string GetPersianDate(DateTime dateTime)
        {
            var persianCalendar = new PersianCalendar();
            var year = persianCalendar.GetYear(dateTime);
            var month = persianCalendar.GetMonth(dateTime);
            return string.Format("{0}/{1}/{2}", year, month, persianCalendar.GetDayOfMonth(dateTime));
        }        
    }
}