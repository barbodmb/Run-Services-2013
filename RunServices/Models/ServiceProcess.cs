using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.ServiceProcess;
using System.Threading.Tasks;

//https://stackoverflow.com/questions/17740048/quartz-net-trigger-does-not-fire-mvc4

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

            return servicesInfo;
        }//end of ServicesNameStatus

        public async Task StartService(string serviceName)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                if (service.Status == ServiceControllerStatus.Stopped)
                {
                    int millisec = Environment.TickCount;
                    TimeSpan timeout = TimeSpan.FromMilliseconds(millisec);
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                }
            }
            catch
            {
                //Console.WriteLine("Can not open Service...");
            }
        }//end of Start

        public async Task StopService(string serviceName)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                if (service.Status == ServiceControllerStatus.Running)
                {
                    var millisec = Environment.TickCount;
                    TimeSpan timeout = TimeSpan.FromMilliseconds(millisec);
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                }
            }
            catch(Exception m)
            {
                //Console.WriteLine("Can not open Service...");
            }
        }//end of Stop

        public async void RestartService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                int millisec1 = Environment.TickCount;
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                await StopService(serviceName);
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                int millisec2 = Environment.TickCount;
                timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));

                await StartService(serviceName);
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch
            {
                // ...
            }
        }//end of REstart

        public List<ConfigService> GetStopServices(List<ConfigService> servicesName)
        {
            var servicesInfo = new List<ConfigService>();
            var sectionDetail = new List<SectionDetail>();
            if (servicesName != null)
            {
                bool requestStatus = false;
                foreach (var item in servicesName)
                {
                    foreach (SectionDetail itemDetail in item.Items.SectionDetail)
                    {
                        if (itemDetail.IntervalUnit == "M")
                        {
                            if (Convert.ToInt32(itemDetail.Interval) / DateTime.Now.Minute == 0
                                && (DateTime.Now.Second <= 10))
                            {
                                requestStatus = SendRequest(itemDetail.Url, itemDetail.RequestTimeOut);
                                if (!requestStatus || item.ServiceStatus == ServiceControllerStatus.Stopped.ToString()
                                                   || item.ServiceStatus == ServiceControllerStatus.StopPending.ToString())
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
                            if (Convert.ToInt32(itemDetail.Interval) / DateTime.Now.Hour == 0
                                && (DateTime.Now.Minute == 0))
                            {
                                var RequestStatus = SendRequest(itemDetail.Url, itemDetail.RequestTimeOut);
                                if (!RequestStatus || item.ServiceStatus == ServiceControllerStatus.Stopped.ToString()
                                                   || item.ServiceStatus == ServiceControllerStatus.StopPending.ToString())
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

            return servicesInfo;
        }

        public async Task RunAutoServices(List<ConfigService> configServices)
        {
            foreach (var item in configServices)
            {
                foreach (SectionDetail itemSectionDetail in item.Items.SectionDetail)
                {
                    if (itemSectionDetail != null && itemSectionDetail.IntervalUnit != "")
                    {
                        if (item.IsPending && (item.ServiceStatus != ServiceControllerStatus.Stopped.ToString()
                                               || item.ServiceStatus != ServiceControllerStatus.StopPending.ToString()))
                        {
                            RestartService(item.ServiceName, 1000);
                        }
                        else
                        {
                            StartService(item.ServiceName);
                        }
                    }
                }
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
    }
}