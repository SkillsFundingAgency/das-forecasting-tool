using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Payments.Messages.PreLoad;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class AllEmployersPaymentPreLoadHttpFunction : IFunction
    {
        [FunctionName("AllEmployersPaymentPreLoadHttpFunction")]
        public static async Task<string> Run(
            [HttpTrigger(AuthorizationLevel.Function,
            "post", Route = "AllEmployersPaymentPreLoadHttpFunction")]HttpRequestMessage req,
            [Queue(QueueNames.PreLoadPayment)] ICollector<PreLoadPaymentMessage> outputQueueMessage,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            return await FunctionRunner.Run<AllEmployersPaymentPreLoadHttpFunction, string>(writer, executionContext,
                async (container, logger) =>
                {
                    var body = await req.Content.ReadAsStringAsync();
                    var preLoadRequest = JsonConvert.DeserializeObject<AllEmployersPreLoadPaymentRequest>(body);

                    return await AllEmployersPaymentDataLoader.QueueEmployersWithNewPayments(preLoadRequest, outputQueueMessage, logger, container);
                });
        }
    }
}
