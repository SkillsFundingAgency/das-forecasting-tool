using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Payments.Messages.PreLoad;
using SFA.DAS.Forecasting.Application.Payments.Services;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.HashingService;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class GetEarningDetailsFunction : IFunction
    {
        [FunctionName("GetEarningDetailsFunction")]
        [return: Queue(QueueNames.CreatePaymentMessage)]
        public static async Task<PreLoadPaymentMessage> Run(
            [QueueTrigger(QueueNames.PreLoadEarningDetailsPayment)]PreLoadPaymentMessage message, 
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            return await FunctionRunner.Run<GetEarningDetailsFunction, PreLoadPaymentMessage>(writer, executionContext,
                async (container, logger) => {

                    // Get ALL EarningDetails from Payment ProviderEventsAPI for a Employer and PeriodId
                    logger.Info($"Running {nameof(GetEarningDetailsFunction)} {message.EmployerAccountId}. {message.PeriodId}");

                    var paymentDataService = container.GetInstance<PaymentApiDataService>();
                    var hashingService = container.GetInstance<IHashingService>();
                    var employerDataService = container.GetInstance<IEmployerDataService>();
					var dataService = container.GetInstance<PreLoadPaymentDataService>();

					var earningDetails = await paymentDataService.PaymentForPeriod(message.PeriodId, message.EmployerAccountId);

                    var hashedAccountId = hashingService.HashValue(message.EmployerAccountId);
                    logger.Info($"Found {earningDetails.Count} for Account: {hashedAccountId}");

                    foreach (var item in earningDetails)
                    {
                        await dataService.StoreEarningDetails(message.EmployerAccountId, item);
                    }

	                var year = 17;
	                var month = 6;

	                var periodIds = await employerDataService.GetPeriodIds();

	                while (year < DateTime.Today.Year % 100 || month < DateTime.Today.Month)
	                {
		                var periodId = periodIds.Where(x => x.CalendarPeriodMonth == month && x.CalendarPeriodYear == year)
			                .Select(x => x.PeriodEndId).FirstOrDefault();

						earningDetails = new List<EarningDetails>();

		                if (!string.IsNullOrWhiteSpace(periodId))
		                {
							earningDetails = await paymentDataService.PaymentForPeriod(message.PeriodId, message.EmployerAccountId);
						}

						foreach (var earningDetail in earningDetails)
						{
							await dataService.StoreEarningDetailsNoCommitment(message.EmployerAccountId, earningDetail);

							logger.Info($"Stored new {nameof(earningDetail)} for {message.EmployerAccountId}");
						}

						month++;
		                if (month > 12)
		                {
			                month = 1;
			                year++;
		                }
	                }


					logger.Info($"Sending message {nameof(message)} to {QueueNames.CreatePaymentMessage}");
                    return message;
                });
        }
    }
}
