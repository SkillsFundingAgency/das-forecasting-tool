using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Handlers;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.ApprenticeshipCourses.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class StoreCourseFunction 
    {
        private readonly IStoreCourseHandler _courseHandler;

        public StoreCourseFunction(IStoreCourseHandler courseHandler)
        {
            _courseHandler = courseHandler;
        }
        [FunctionName("StoreCourseFunction")]
        public async Task Run([QueueTrigger(QueueNames.StoreCourse)] ApprenticeshipCourse course, ILogger log)
        {
            
            log.LogInformation("Received reques to store course");
            
            await _courseHandler.Handle(course);
            log.LogInformation("Finished handling the request to store the course.");
        
        }
    }
}