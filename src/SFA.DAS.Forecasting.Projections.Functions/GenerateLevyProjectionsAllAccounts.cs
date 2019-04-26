using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Projections.Functions
{
    public class GenerateLevyProjectionsAllAccounts :IFunction
    {
        [FunctionName("GenerateLevyProjectionsAllAccounts")]
        [return: Queue(QueueNames.GenerateProjectionForAllAccounts)]
        public static string Run([TimerTrigger("0 0 0 25 * *")]TimerInfo myTimer, TraceWriter log)
        {
            return Enum.GetName(typeof(ProjectionSource), ProjectionSource.LevyDeclaration);
        }
    }
}