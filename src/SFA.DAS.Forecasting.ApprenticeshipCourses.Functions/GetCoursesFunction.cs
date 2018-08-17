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
    public class GetCoursesFunction : IFunction
    {
        [FunctionName("GetCoursesFunction")]
        public static async Task Run([QueueTrigger(QueueNames.GetCourses)] RefreshCourses message,
            TraceWriter log,
            [Queue(QueueNames.StoreCourse)]ICollector<ApprenticeshipCourse> storeCourses,
            ExecutionContext executionContext)
        {
            await FunctionRunner.Run<GetCoursesFunction>(log, executionContext, async (container, logger) =>
            {
                logger.Debug("Starting GetCoursesFunction Function.");
                var handler = container.GetInstance<GetCoursesHandler>();
                var courses = await handler.Handle(message);
                courses.ForEach(storeCourses.Add);
                log.Info($"Finished getting courses. Got {courses.Count} courses.");
            });
        }
    }
}