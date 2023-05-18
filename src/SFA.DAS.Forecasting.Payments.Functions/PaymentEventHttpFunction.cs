using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Payments.Messages;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public class PaymentEventHttpFunction 
    {
        [FunctionName("PaymentEventHttpFunction")]
        [return: Queue(QueueNames.PaymentValidator)]
        public async Task<PaymentCreatedMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "EmployerPaymentEventHttpFunction")]HttpRequestMessage req,
            ILogger logger)
        {
            var body = await req.Content.ReadAsStringAsync();
            var paymentEvent = JsonConvert.DeserializeObject<PaymentCreatedMessage>(body);

            logger.LogInformation($"Added one payment to {QueueNames.PaymentProcessor} queue.");
            return await Task.FromResult(paymentEvent);
            
        }
    }
}
