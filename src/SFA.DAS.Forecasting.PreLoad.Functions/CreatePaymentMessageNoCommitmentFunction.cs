using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Payments.Messages.PreLoad;
using SFA.DAS.Forecasting.Application.Payments.Services;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Models.Payments;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class CreatePaymentMessageNoCommitmentFunction : IFunction
    {
        [FunctionName("CreatePaymentMessageNoCommitmentFunction")]
        [return: Queue(QueueNames.RemovePreLoadDataNoCommitment)]
        public static PreLoadPaymentMessage Run(
            [QueueTrigger(QueueNames.CreatePaymentMessageNoCommitment)]PreLoadPaymentMessage message,
            [Queue(QueueNames.PaymentValidatorNoCommitment)] ICollector<PaymentCreatedMessage> noCommitmentOutputQueueMessage,
			ExecutionContext executionContext,
            TraceWriter writer)
        {
            return FunctionRunner.Run<CreatePaymentMessageNoCommitmentFunction, PreLoadPaymentMessage>(writer, executionContext,
                (container, logger) =>
                {
                    logger.Info($"{nameof(CreatePaymentMessageFunction)} started");

                    var dataService = container.GetInstance<PreLoadPaymentDataService>();

                    var paymentsNoCommitments = dataService.GetPaymentsNoCommitment(message.EmployerAccountId).ToList();
	                var earningDetailsNocommitments = dataService.GetEarningDetailsNoCommitment(message.EmployerAccountId).ToList();

					logger.Info($"Got {paymentsNoCommitments.Count()} payments to match against {earningDetailsNocommitments.Count} earning details for employer '{message.EmployerAccountId}'");

					
		            var paymentNoCommitmentCreatedMessage =
						paymentsNoCommitments
							.Select(p => CreatePayment(logger, p, earningDetailsNocommitments))
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
        
        public static PaymentCreatedMessage CreatePayment(ILog logger, EmployerPayment payment, IEnumerable<EarningDetails> earningDetails)
        {
            if (payment == null)
            {
                logger.Warn("No payment passed to CreatePaymentSubstituteData");
                return null;
            }
            var earningDetail = earningDetails.FirstOrDefault(ed => Guid.TryParse(ed.PaymentId, out Guid paymentGuid) && paymentGuid == payment.PaymentId);
            if (earningDetail == null)
            {
                logger.Warn($"No earning details found for payment: {payment.PaymentId}, apprenticeship: {payment.ApprenticeshipId}");
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
                EarningDetails = earningDetail,
                FundingSource = payment.FundingSource,
                SendingEmployerAccountId = payment.SenderAccountId ?? payment.AccountId
            };
        }
    }
}
