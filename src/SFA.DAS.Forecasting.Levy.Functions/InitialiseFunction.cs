using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Functions.Framework.Infrastructure;

namespace SFA.DAS.Forecasting.Levy.Functions
{
    public class InitialiseFunction: IFunction
    {
        [FunctionName("InitialiseFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req,
            ExecutionContext executionContext,
            TraceWriter log)
        {
            await FunctionRunner.Run<InitialiseFunction>(log, executionContext, async (container,logger) =>
            {
				//TODO: create generic function or use custom binding
				var telemetry = container.GetInstance<IAppInsightsTelemetry>();

	            telemetry.Info("RefreshStandardsHttpFunction", "Initialising the Levy functions.", "FunctionRunner.Run", executionContext.InvocationId);

                await container.GetInstance<IFunctionInitialisationService>().Initialise<InitialiseFunction>();
	            telemetry.Info("RefreshStandardsHttpFunction", "Finished initialising the Levy functions.", "FunctionRunner.Run", executionContext.InvocationId);
            });

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
