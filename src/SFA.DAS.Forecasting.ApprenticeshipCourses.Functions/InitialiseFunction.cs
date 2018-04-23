using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Functions.Framework.Infrastructure;
using SFA.DAS.Forecasting.Messages.ApprenticeshipCourses;

namespace SFA.DAS.Forecasting.ApprenticeshipCourses.Functions
{
    public class InitialiseFunction : IFunction
    {
        [FunctionName("InitialiseFunction")]
        [return: Queue(QueueNames.GetStandards)]
        public static async Task<RefreshCourses> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req,
            ExecutionContext executionContext,
            TraceWriter log)
        {
            return await FunctionRunner.Run<InitialiseFunction, RefreshCourses>(log, executionContext, async (container, logger) =>
             {
                //TODO: create generic function or use custom binding
                log.Info("Initialising the Levy functions.");
                 await container.GetInstance<IFunctionInitialisationService>().Initialise<InitialiseFunction>();
                 log.Info("Finished initialising the Levy functions.");
                 return new RefreshCourses { RequestTime = DateTime.Now };
             });
        }
    }
}
