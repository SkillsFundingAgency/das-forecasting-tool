using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Handlers;
using SFA.DAS.Forecasting.Messages.ApprenticeshipCourses;
using SFA.DAS.Forecasting.Models.Estimation;
using ExecutionContext = System.Threading.ExecutionContext;

namespace SFA.DAS.Forecasting.ApprenticeshipCourses.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class GetCoursesFunction
    {
        private readonly IGetCoursesHandler _handler;

        public GetCoursesFunction(IGetCoursesHandler handler)
        {
            _handler = handler;
        }
        [FunctionName("GetCoursesFunction")]
        public async Task RunAsync(
            [QueueTrigger(QueueNames.GetCourses)] RefreshCourses message,
            ILogger logger,
            [Queue(QueueNames.StoreCourse)] ICollector<ApprenticeshipCourse> storeCourses)
        {
            
                logger.LogInformation("Starting GetCoursesFunction Function.");
                var courses = await _handler.Handle(message);
                courses.ForEach(storeCourses.Add);
                logger.LogInformation($"Finished getting courses. Got {courses.Count} courses.");
            
        }
    }
}