using StructureMap;
using System;
using System.IO;
using System.Reflection;
using SFA.DAS.Forecasting.Application.Infrastructure.Registries;

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
                    c.AddRegistry<ConfigurationRegistry>();
                    c.AddRegistry<DefaultRegistry>();
                    c.AddRegistry<MediatrRegistry>();
                    var binPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    c.Scan(assScanner =>
                    {
                        //assScanner.LookForRegistries();
                        assScanner.TheCallingAssembly();
                        assScanner.AssembliesFromPath(binPath, a => a.GetName().Name.StartsWith("SFA.DAS.Forecasting"));
                        assScanner.RegisterConcreteTypesAgainstTheFirstInterface();
                    });
                }));
            }
        }
    }
}
