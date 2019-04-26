using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Projections.Functions
{
    public class GeneratePaymentProjectionsAllAccounts : IFunction
    {
        [FunctionName("GeneratePaymentProjectionsAllAccounts")]
        [return: Queue(QueueNames.GenerateProjectionForAllAccounts)]
        public static string Run([TimerTrigger("0 0 0 15 * *")]TimerInfo myTimer, TraceWriter log)
        {
            return Enum.GetName(typeof(ProjectionSource), ProjectionSource.PaymentPeriodEnd);
        }
    }
}