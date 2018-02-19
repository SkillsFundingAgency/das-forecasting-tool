using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Payments.Services;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Models.Payments;
using SFA.DAS.HashingService;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public class CreatePaymentMessageFunction : IFunction
    {
        [FunctionName("CreatePaymentMessageFunction")]
        public static async Task Run(
            [QueueTrigger(QueueNames.AddEarningDetails)]PreLoadMessage message,
            [Queue(QueueNames.PaymentValidator)] ICollector<PaymentCreatedMessage> outputQueueMessage,
            TraceWriter writer)
        {
            await FunctionRunner.Run<CreatePaymentMessageFunction>(writer,
                async (container, logger) =>
                {
                    logger.Info($"{nameof(CreatePaymentMessageFunction)} started");

                    var dataService = container.GetInstance<PreLoadPaymentDataService>();
                    var payments = dataService.GetPayments(message.EmployerAccountId);
                    var earningDetails = dataService.GetEarningDetails(message.EmployerAccountId);

                    var paymentCreatedMessage = payments
                        .Select(p => CreatePayment(p, earningDetails))
                        .Where(p => p != null);

                    foreach (var p in paymentCreatedMessage)
                    {
                        outputQueueMessage.Add(p);
                    }

                    // ToDo: Delete from TS
                    logger.Info($"{nameof(CreatePaymentMessageFunction)} finished, Payments created: {paymentCreatedMessage.Count()}");

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
                CollectionPeriod = new Application.Payments.Messages.CollectionPeriod { Id = payment.CollectionPeriodId, Year = payment.CollectionPeriodYear, Month = payment.CollectionPeriodMonth },
                EarningDetails = ed,
                
            };
        }
    }
}
