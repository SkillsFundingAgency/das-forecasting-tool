using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Functions.Framework.Infrastructure;

namespace SFA.DAS.Forecasting.Payments.Functions
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
	            var telemetry = container.GetInstance<IAppInsightsTelemetry>();

				//TODO: create generic function or use custom binding
				telemetry.Info("InitialiseFunction", "Initialising the Payments functions.", "FunctionRunner.Run", executionContext.InvocationId);

				await container.GetInstance<IFunctionInitialisationService>().Initialise<InitialiseFunction>();
                telemetry.Info("InitialiseFunction", "Finished initialising the Payments functions.", "FunctionRunner.Run", executionContext.InvocationId);
            });

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
