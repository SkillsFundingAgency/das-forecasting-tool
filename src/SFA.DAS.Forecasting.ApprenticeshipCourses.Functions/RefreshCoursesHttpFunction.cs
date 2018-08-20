using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Messages.ApprenticeshipCourses;

namespace SFA.DAS.Forecasting.ApprenticeshipCourses.Functions
{

    public class RefreshCoursesHttpFunction : IFunction
    {
        [FunctionName("RefreshCoursesHttpFunction")]
        [return: Queue(QueueNames.GetCourses)]
        public static async Task<RefreshCourses> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "RefreshCoursesHttpFunction")]HttpRequestMessage req,
            TraceWriter writer, ExecutionContext executionContext)
        {
            return await FunctionRunner.Run<RefreshCoursesHttpFunction, RefreshCourses>(writer, executionContext,
                async (container, logger) =>
                {
                    var body = await req.Content.ReadAsStringAsync();
                    var msg = JsonConvert.DeserializeObject<RefreshMessage>(body);

                    logger.Info($"Received refresh for {msg.CourseType} request.");
                    return new RefreshCourses{
                        RequestTime = DateTime.Now,
                        CourseType = msg.CourseType
                    };
                });
        }
    }
}