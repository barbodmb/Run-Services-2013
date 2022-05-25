using Microsoft.Practices.Unity;
using Quartz;
using Quartz.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Unity.Mvc4;

namespace RunServices.Job
{
    public static class Bootstrapper
    {
        public static IUnityContainer Initialise()
        {
            var container = BuildUnityContainer();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            return container;
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            container.RegisterType<ISchedulerFactory, UnitySchedulerFactory>(new ContainerControlledLifetimeManager());
            container.RegisterType<IScheduler>(new InjectionFactory(c => c.Resolve<ISchedulerFactory>().GetScheduler()));
            container.RegisterType<IQuartzScheduler, QuartzScheduler>(new ContainerControlledLifetimeManager());
            container.RegisterType<IEvaluationBus, EvaluationBus>();

            RegisterTypes(container);

            return container;
        }

        public static void RegisterTypes(IUnityContainer container)
        {
        }
    }
}