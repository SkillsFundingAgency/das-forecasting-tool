using StructureMap;
using System;

namespace SFA.DAS.Forecasting.Functions.Framework.Infrastructure
{
    public class ContainerBootstrapper : Registry
    {
        private static readonly object LockObject = new object();
        private static IContainer container;
        public static IContainer Bootstrap()
        {
            lock (LockObject)
            {
                return container ?? (container = new Container(c =>
                {
                    c.Scan(o => {
                        o.LookForRegistries();
                        o.AssembliesFromPath(Environment.CurrentDirectory);
                        o.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS"));
                        o.RegisterConcreteTypesAgainstTheFirstInterface();
                    });
                }));
            }
        }
    }
}
