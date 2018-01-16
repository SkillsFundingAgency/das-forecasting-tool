using System;
using SFA.DAS.Forecasting.Payments.Application.Infrastructure.Registries;
using SFA.DAS.Messaging.POC.Infrastructure.Registries;
using StructureMap;

namespace SFA.DAS.Forecasting.Payments.Functions.Infrastructure
{
    public class ContainerBootstrapper : Registry
    {
        private static readonly object lockObject = new object();
        private static IContainer container; 
        public static IContainer Bootstrap()
        {
            lock (lockObject)
            {
                return container ?? (container = new Container(c =>
                {
                    c.AddRegistry<DefaultRegistry>();
                    c.AddRegistry<MessagingRegistry>();
                }));
            }
        }
    }
}