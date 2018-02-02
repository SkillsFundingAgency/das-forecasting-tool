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
        [return: Queue(QueueNames.PaymentProcessor)]
        public static async Task<PaymentEvent> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "EmployerPaymentEventHttpFunction")]HttpRequestMessage req, 
            TraceWriter writer)
        {
            return await FunctionRunner.Run<PaymentEventHttpFunction, PaymentEvent>(writer,
                async (container, logger) =>
                {
                    var body = await req.Content.ReadAsStringAsync();
                    var paymentEvent = JsonConvert.DeserializeObject<PaymentEvent>(body);

                    logger.Info($"Added one payment to {QueueNames.PaymentProcessor} queue.");
                    return await Task.FromResult(paymentEvent);
                });
        }
    }
}
