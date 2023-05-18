using System;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Messages.ApprenticeshipCourses;

namespace SFA.DAS.Forecasting.ApprenticeshipCourses.Functions
{

    public class RefreshStandardsHttpFunction 
    {
        [FunctionName("RefreshStandardsHttpFunction")]
        [return: Queue(QueueNames.GetCourses)]
        public static RefreshCourses Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "RefreshStandardsHttpFunction")]HttpRequestMessage req,
            ILogger logger)
        {
            
            logger.LogInformation("Received refresh standards request.");
            return new RefreshCourses{
                RequestTime = DateTime.Now,
                CourseType = CourseType.Standards
            };
        }
    }
}