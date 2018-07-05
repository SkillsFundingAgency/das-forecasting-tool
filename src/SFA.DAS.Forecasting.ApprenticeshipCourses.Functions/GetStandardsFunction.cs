using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Handlers;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Messages.ApprenticeshipCourses;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.ApprenticeshipCourses.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class GetStandardsFunction: IFunction
    {
        [FunctionName("GetStandardsFunction")]
        public static async Task Run([QueueTrigger(QueueNames.GetStandards)] RefreshCourses message,
            TraceWriter log,
            [Queue(QueueNames.StoreCourse)]ICollector<ApprenticeshipCourse> storeStandards,
            ExecutionContext executionContext)
        {
            await FunctionRunner.Run<GetStandardsFunction>(log, executionContext, async (container, logger) =>
            {
                //TODO: create generic function or use custom binding
                logger.Debug("Starting GetStandards Function.");
                var handler = container.GetInstance<GetStandardsHandler>();
                var courses = await handler.Handle(message);
                courses.ForEach(storeStandards.Add);
                log.Info($"Finished getting standards. Got {courses.Count} courses.");
            });
        }
    }
}