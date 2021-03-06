using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Application.Payments.Messages;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public class PaymentEventHttpFunction : IFunction
    {
        [FunctionName("PaymentEventHttpFunction")]
        [return: Queue(QueueNames.PaymentValidator)]
        public static async Task<PaymentCreatedMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "EmployerPaymentEventHttpFunction")]HttpRequestMessage req,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            return await FunctionRunner.Run<PaymentEventHttpFunction, PaymentCreatedMessage>(writer, executionContext,
                async (container, logger) =>
                {
                    var body = await req.Content.ReadAsStringAsync();
                    var paymentEvent = JsonConvert.DeserializeObject<PaymentCreatedMessage>(body);

                    logger.Info($"Added one payment to {QueueNames.PaymentProcessor} queue.");
                    return await Task.FromResult(paymentEvent);
                });
        }
    }
}
