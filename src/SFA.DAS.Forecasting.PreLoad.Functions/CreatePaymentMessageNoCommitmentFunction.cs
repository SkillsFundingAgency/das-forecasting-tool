using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Payments.Messages.PreLoad;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Models.Payments;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class CreatePaymentMessageNoCommitmentFunction : IFunction
    {
        [FunctionName("CreatePaymentMessageNoCommitmentFunction")]
        [return: Queue(QueueNames.RemovePreLoadDataNoCommitment)]
        public static async Task<PreLoadPaymentMessage> Run(
            [QueueTrigger(QueueNames.CreatePaymentMessageNoCommitment)]PreLoadPaymentMessage message,
            [Queue(QueueNames.PaymentValidatorNoCommitment)] ICollector<PaymentCreatedMessage> noCommitmentOutputQueueMessage,
			ExecutionContext executionContext,
            TraceWriter writer)
        {
            return await FunctionRunner.Run<CreatePaymentMessageNoCommitmentFunction, PreLoadPaymentMessage>(writer, executionContext,
                async (container, logger) =>
                {
                    logger.Info($"{nameof(CreatePaymentMessageFunction)} started");

                    var dataService = container.GetInstance<IEmployerDatabaseService>();

                    var payments = await dataService.GetPastEmployerPayments(message.EmployerAccountId,message.PeriodYear, message.PeriodMonth);

                    logger.Info($"Got {payments.Count} payments for employer '{message.EmployerAccountId}'");

					
		            var paymentNoCommitmentCreatedMessage =
		                payments
                            .Select(p => CreatePayment(logger, p))
				            .Where(p => p != null)
				            .ToList();
	            

	                foreach (var p in paymentNoCommitmentCreatedMessage)
	                {
		                noCommitmentOutputQueueMessage.Add(p);
	                }

	                logger.Info($"{nameof(CreatePaymentMessageNoCommitmentFunction)} finished, Past payments created: {paymentNoCommitmentCreatedMessage.Count}");

					return message;
                });
        }
        
        public static PaymentCreatedMessage CreatePayment(ILog logger, EmployerPayment payment)
        {
            if (payment == null)
            {
                logger.Warn("No payment passed to PaymentCreatedMessage");
                return null;
            }

            return new PaymentCreatedMessage
            {
                Id = payment.PaymentId.ToString(),
                EmployerAccountId = payment.AccountId,
                Ukprn = payment.Ukprn,
                ApprenticeshipId = payment.ApprenticeshipId,
                Amount = payment.Amount,
                ProviderName = payment.ProviderName,
                ApprenticeName = payment.ApprenticeName,
                CourseName = payment.ApprenticeshipCourseName,
                CourseLevel = payment.ApprenticeshipCourseLevel,
                Uln = payment.Uln,
                CourseStartDate = payment.ApprenticeshipCourseStartDate,
                CollectionPeriod = new Application.Payments.Messages.NamedCalendarPeriod { Id = payment.CollectionPeriodId, Year = payment.CollectionPeriodYear, Month = payment.CollectionPeriodMonth },
                DeliveryPeriod = new Application.Payments.Messages.CalendarPeriod { Month = payment.DeliveryPeriodMonth, Year = payment.DeliveryPeriodYear },
                FundingSource = payment.FundingSource,
                SendingEmployerAccountId = payment.SenderAccountId ?? payment.AccountId
            };
        }
    }
}
