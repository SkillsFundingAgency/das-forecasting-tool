using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Payments.Messages;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public class PaymentEventNoCommitmentHttpFunction
    {
        [FunctionName("PaymentEventNoCommitmentHttpFunction")]
        [return: Queue(QueueNames.PaymentValidatorNoCommitment)]
        public static async Task<PaymentCreatedMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "EmployerPaymentEventNoCommitmentHttpFunction")]HttpRequestMessage req,
            ILogger logger)
        {
            var body = await req.Content.ReadAsStringAsync();
            var paymentEvent = JsonConvert.DeserializeObject<PaymentCreatedMessage>(body);

            logger.LogInformation($"Added one payment to {QueueNames.PaymentProcessor} queue.");
            return await Task.FromResult(paymentEvent);
        }
    }
}
