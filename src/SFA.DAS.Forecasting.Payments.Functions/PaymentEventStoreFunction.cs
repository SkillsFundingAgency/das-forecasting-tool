using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Levy.Handlers;
using SFA.DAS.Forecasting.Application.Payments.Handlers;
using SFA.DAS.Forecasting.Application.Payments.Mapping;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Shared.Queues;
using SFA.DAS.Forecasting.Domain.Payments.Aggregates;
using SFA.DAS.Forecasting.Functions.Framework;
using CollectionPeriod = SFA.DAS.Forecasting.Application.Payments.Messages.CollectionPeriod;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public class PaymentEventStoreFunction : IFunction
    {
	    public EarningDetails EarningDetails { get; set; }
	    public CollectionPeriod CollectionPeriod { get; set; }
		[FunctionName("PaymentEventStoreFunction")]
        public static async Task Run(
            [QueueTrigger(QueueNames.PaymentProcessor)]PaymentEvent paymentEvent, 
            TraceWriter writer)
        {
            await FunctionRunner.Run<PaymentEventStoreFunction, int>(writer,
                async (container, logger) =>
                {
	                logger.Debug("Getting levy declaration handler from container.");
	                var handler = container.GetInstance<ProcessEmployerPaymentHandler>();
	                var mapper = container.GetInstance<PaymentMapper>();

					await handler.Handle(mapper.MapToPayment(paymentEvent));

                    return await Task.FromResult(1);
                });
        }
    }
}
