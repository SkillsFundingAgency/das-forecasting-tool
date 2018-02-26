using StructureMap;
using System;

namespace SFA.DAS.Forecasting.Functions.Framework.Infrastructure
{
    public class ContainerBootstrapper : Registry
    {
        private static readonly object LockObject = new object();
        private static IContainer _container;
        public static IContainer Bootstrap()
        {
            lock (LockObject)
            {
                return _container ?? (_container = new Container(c =>
                {
                    c.Scan(o => {
                        o.LookForRegistries();
                        o.TheCallingAssembly();
                        o.AssembliesFromPath(Environment.CurrentDirectory, a => a.GetName().Name.StartsWith("SFA.DAS.Forecasting"));
                        o.RegisterConcreteTypesAgainstTheFirstInterface();
                    });
                }));
            }
        }
    }
}
