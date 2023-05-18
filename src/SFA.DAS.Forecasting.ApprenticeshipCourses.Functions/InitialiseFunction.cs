using System;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Messages.ApprenticeshipCourses;

namespace SFA.DAS.Forecasting.ApprenticeshipCourses.Functions
{
    public class InitialiseFunction 
    {
        [FunctionName("InitialiseFunction")]
        [return: Queue(QueueNames.GetCourses)]
        public RefreshCourses Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req,
            ILogger logger)
        {
            return new RefreshCourses {
                 RequestTime = DateTime.Now,
                 CourseType = CourseType.Standards | CourseType.Frameworks
             };
        
        }
    }
}
