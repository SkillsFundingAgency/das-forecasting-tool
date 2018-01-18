using StructureMap;
using System;

namespace SFA.DAS.Forecasting.Functions.Framework.Infrastructure
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
                    c.Scan(o => {
                        o.AssembliesFromPath(Environment.CurrentDirectory);
                        o.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS"));
                        o.RegisterConcreteTypesAgainstTheFirstInterface();
                    });
                    //c.AddRegistry<DefaultRegistry>();
                    //c.AddRegistry<MessagingRegistry>();
                }));
            }
        }
    }
}
