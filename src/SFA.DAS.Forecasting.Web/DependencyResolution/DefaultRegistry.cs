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
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.Forecasting.Web.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.Forecasting";
        private const string ServiceNamespace = "SFA.DAS";

        public DefaultRegistry()
        {
            Scan(
                scan =>
                {
                    scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(ServiceNamespace));
                    scan.TheCallingAssembly();
                    scan.RegisterConcreteTypesAgainstTheFirstInterface();
                    scan.AssemblyContainingType<Ping>();
                });

            ConfigureLogging();
        }

        private void ConfigureLogging()
        {
            For<ILog>().Use(x => new NLogLogger(
                x.ParentType,
                null,
                null)).AlwaysUnique();
        }
    }
}