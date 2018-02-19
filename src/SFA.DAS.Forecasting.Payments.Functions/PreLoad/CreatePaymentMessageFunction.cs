using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Payments.Services;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.Payments.Functions.PreLoad
{
    public class CreatePaymentMessageFunction : IFunction
    {
        [FunctionName("CreatePaymentMessageFunction")]
        public static void Run(
            [QueueTrigger(QueueNames.AddEarningDetails)]PreLoadMessage message,
            [Queue(QueueNames.PaymentValidator)] ICollector<PaymentCreatedMessage> outputQueueMessage,
            TraceWriter writer)
        {
            FunctionRunner.Run<CreatePaymentMessageFunction>(writer, (container, logger) =>
                {
                    logger.Info($"{nameof(CreatePaymentMessageFunction)} started");

                    var dataService = container.GetInstance<PreLoadPaymentDataService>();
                    var payments = dataService.GetPayments(message.EmployerAccountId);
                    var earningDetails = dataService.GetEarningDetails(message.EmployerAccountId);

                    var paymentCreatedMessage = payments
                        .Select(p => CreatePayment(p, earningDetails))
                        .Where(p => p != null)
                        .ToList();

                    foreach (var p in paymentCreatedMessage)
                    {
                        outputQueueMessage.Add(p);
                    }

                    // ToDo: Delete from TS
                    logger.Info($"{nameof(CreatePaymentMessageFunction)} finished, Payments created: {paymentCreatedMessage.Count}");
                });
        }

        public static PaymentCreatedMessage CreatePayment(EmployerPayment payment, IEnumerable<EarningDetails> earningDetails)
        {
            var ed = earningDetails.FirstOrDefault(m => m.RequiredPaymentId == payment.PaymentId);
            if (payment == null || ed == null)
                return null;

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
                CollectionPeriod = new CollectionPeriod { Id = payment.CollectionPeriodId, Year = payment.CollectionPeriodYear, Month = payment.CollectionPeriodMonth },
                EarningDetails = ed
            };
        }
    }
}
