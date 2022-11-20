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

using System.Net.Http;
using System.Net.NetworkInformation;
using System.Web;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using SFA.DAS.Forecasting.Application.Estimations.Services;
using SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;
using SFA.DAS.Forecasting.Application.Infrastructure.Registries;
using SFA.DAS.Forecasting.Domain.Estimations;
using SFA.DAS.Forecasting.Web.Authentication;
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
                    scan.AssemblyContainingType<AccountEstimation>();
                    scan.AssemblyContainingType<AccountEstimationDataService>();
                    scan.RegisterConcreteTypesAgainstTheFirstInterface();
                    scan.AssemblyContainingType<Ping>();
                    scan.Exclude(c=>c == typeof(IApiClient));
                    
                });

            For<IApiClient>()
                .Use<ApiClient>()
                .Ctor<HttpClient>()
                .Is(new HttpClient());

            ConfigureLogging();

            For<HttpContextBase>().Use(() => new HttpContextWrapper(HttpContext.Current));
            
            For<IMembershipService>().Use<MembershipService>();
            var config =
                new TelemetryConfiguration(ConfigurationHelper.GetAppSetting("APPINSIGHTS_INSTRUMENTATIONKEY", false));
            var client = new TelemetryClient(config);
            client.InstrumentationKey = config.InstrumentationKey;

            
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