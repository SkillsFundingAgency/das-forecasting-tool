using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Handlers;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.ApprenticeshipCourses.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class StoreCourseFunction : IFunction
    {
        [FunctionName("StoreCourseFunction")]
        public static async Task Run([QueueTrigger(QueueNames.StoreCourse)] ApprenticeshipCourse course,
            TraceWriter log,
            ExecutionContext executionContext)
        {
            await FunctionRunner.Run<StoreCourseFunction>(log, executionContext, async (container, logger) =>
            {
	            var telemetry = container.GetInstance<IAppInsightsTelemetry>();

	            telemetry.TrackEvent("StoreCourseFunction", "Received request to store course", "FunctionRunner.Run", executionContext.InvocationId);

                var handler = container.GetInstance<StoreCourseHandler>();
                await handler.Handle(course);
	            telemetry.TrackEvent("StoreCourseFunction", "Finished handling the request to store the course", "FunctionRunner.Run", executionContext.InvocationId);
            });
        }
    }
}