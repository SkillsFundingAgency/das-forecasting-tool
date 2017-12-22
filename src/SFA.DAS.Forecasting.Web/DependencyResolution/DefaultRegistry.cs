// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRegistry.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Net.NetworkInformation;
using System.Web;
using System.Web.Routing;

using MediatR;

using SFA.DAS.Forecasting.Domain.Interfaces;
using SFA.DAS.Forecasting.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Infrastructure.Repositories;
using SFA.DAS.NLog.Logger;

using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap;

namespace SFA.DAS.Forecasting.Web.DependencyResolution {
	
    public class DefaultRegistry : Registry {

        private const string ServiceNamespace = "SFA.DAS";

        public DefaultRegistry() {
            Scan(
                scan => {
                    scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(ServiceNamespace));
                    scan.TheCallingAssembly();
                    scan.RegisterConcreteTypesAgainstTheFirstInterface();                    
                    scan.AssemblyContainingType<Ping>();
                    scan.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<>));
                    scan.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                    scan.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                    //scan.TheCallingAssembly();
                    //scan.WithDefaultConventions();
                });

            ConfigureLogging();

            
            RegisterMediator();
        }


        private void ConfigureLogging()
        {
            For<ILog>().Use(x => new NLogLogger(
                x.ParentType,
                null,
                null)).AlwaysUnique();
        }

        private void RegisterMediator()
        {
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
            For<IMediator>().Use<Mediator>();
        }
    }
}