// using Microsoft.ApplicationInsights;
// using Microsoft.ApplicationInsights.Extensibility;
// using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
// using StructureMap;
//
// namespace SFA.DAS.Forecasting.Application.Infrastructure.Registries
// {
// 	public class TelemetryRegistry : Registry
// 	{
// 		public TelemetryRegistry()
// 		{
// 		    var config =
// 		        new TelemetryConfiguration(ConfigurationHelper.GetAppSetting("APPINSIGHTS_INSTRUMENTATIONKEY", false));
//             var client = new TelemetryClient(config);
// 		    client.InstrumentationKey = config.InstrumentationKey;
//
// 		    //ForSingletonOf<TelemetryConfiguration>().Use<TelemetryConfiguration>()
// 		    //    .Ctor<string>(ConfigurationHelper.GetAppSetting("APPINSIGHTS_INSTRUMENTATIONKEY", false));
// 		    //ForSingletonOf<TelemetryClient>().Use<TelemetryClient>().Ctor<TelemetryConfiguration>().Is(ctx => ctx.GetInstance<TelemetryConfiguration>());
// 		    ForSingletonOf<TelemetryConfiguration>().Use(config);
// 		    ForSingletonOf<TelemetryClient>().Use(client);
// 		}
// 	}
// }
