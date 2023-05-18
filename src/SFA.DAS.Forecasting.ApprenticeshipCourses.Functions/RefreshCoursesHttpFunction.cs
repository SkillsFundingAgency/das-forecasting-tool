using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.ApprenticeshipCourses.Functions.Messages;
using SFA.DAS.Forecasting.Messages.ApprenticeshipCourses;

namespace SFA.DAS.Forecasting.ApprenticeshipCourses.Functions
{

    public class RefreshCoursesHttpFunction
    {
        [FunctionName("RefreshCoursesHttpFunction")]
        [return: Queue(QueueNames.GetCourses)]
        public async Task<RefreshCourses> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "RefreshCoursesHttpFunction")]HttpRequestMessage req,
            ILogger logger)
        {
            var body = await req.Content.ReadAsStringAsync();
            var msg = JsonConvert.DeserializeObject<RefreshMessage>(body);

            logger.LogInformation($"Received refresh for {msg.CourseType} request.");
            return new RefreshCourses{
                RequestTime = DateTime.Now,
                CourseType = msg.CourseType
            };
        }
    }
}