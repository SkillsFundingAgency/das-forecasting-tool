using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Functions.Framework.Infrastructure;
using SFA.DAS.Forecasting.Messages.ApprenticeshipCourses;

namespace SFA.DAS.Forecasting.ApprenticeshipCourses.Functions
{
    public class InitialiseFunction : IFunction
    {
        [FunctionName("InitialiseFunction")]
        [return: Queue(QueueNames.GetStandards)]
        public static async Task<RefreshCourses> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req,
            ExecutionContext executionContext,
            TraceWriter log)
        {
            return await FunctionRunner.Run<InitialiseFunction, RefreshCourses>(log, executionContext, async (container, logger) =>
             {
				 //TODO: create generic function or use custom binding
				 var telemetry = container.GetInstance<IAppInsightsTelemetry>();

	             telemetry.Info("RefreshStandardsHttpFunction", "Initialising the Apprenticeship Courses function application.", "FunctionRunner.Run", executionContext.InvocationId);

                 await container.GetInstance<IFunctionInitialisationService>()
                     .Initialise<InitialiseFunction>();
	             telemetry.Info("RefreshStandardsHttpFunction", "Finished initialising the Apprenticeship Courses function application.", "FunctionRunner.Run", executionContext.InvocationId);
                 return new RefreshCourses { RequestTime = DateTime.Now };
             });
        }
    }
}
