using System;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Messages.ApprenticeshipCourses;

namespace SFA.DAS.Forecasting.ApprenticeshipCourses.Functions
{

    public class RefreshStandardsHttpFunction : IFunction
    {
        [FunctionName("RefreshStandardsHttpFunction")]
        [return: Queue(QueueNames.GetStandards)]
        public static RefreshCourses Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "RefreshStandardsHttpFunction")]HttpRequestMessage req,
            TraceWriter writer, ExecutionContext executionContext)
        {
            return FunctionRunner.Run<RefreshStandardsHttpFunction, RefreshCourses>(writer, executionContext,
                (container, logger) =>
                {
	                var telemetry = container.GetInstance<IAppInsightsTelemetry>();

	                telemetry.Info("RefreshStandardsHttpFunction", "Received refresh standards request.", "FunctionRunner.Run", executionContext.InvocationId);

                    return new RefreshCourses{RequestTime = DateTime.Now};
                });
        }
    }
}