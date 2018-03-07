using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Payments.Messages.PreLoad;
using SFA.DAS.Forecasting.Application.Payments.Services;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class CreatePaymentMessageFunction : IFunction
    {
        [FunctionName("CreatePaymentMessageFunction")]
        public static async Task Run(
            [QueueTrigger(QueueNames.AddEarningDetails)]PreLoadPaymentMessage message,
            [Queue(QueueNames.PaymentValidator)] ICollector<PaymentCreatedMessage> outputQueueMessage,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            await FunctionRunner.Run<CreatePaymentMessageFunction>(writer, executionContext,
                async (container, logger) =>
                {
                    logger.Info($"{nameof(CreatePaymentMessageFunction)} started");

                    var dataService = container.GetInstance<PreLoadPaymentDataService>();
                    var payments = dataService.GetPayments(message.EmployerAccountId);
                    var earningDetails = dataService.GetEarningDetails(message.EmployerAccountId);

                    List<PaymentCreatedMessage> paymentCreatedMessage;
                    if (message.SubstitutionId != null)
                    {
                        paymentCreatedMessage =
                            payments
                            .Select(p => CreatePaymentSubstituteData(p, earningDetails, message.SubstitutionId))
                            .Where(p => p != null)
                            .ToList();
                    }
                    else {
                        paymentCreatedMessage = 
                            payments
                            .Select(p => CreatePayment(p, earningDetails))
                            .Where(p => p != null)
                            .ToList();
                    }

                    foreach (var p in paymentCreatedMessage)
                    {
                        outputQueueMessage.Add(p);
                    }

                    await dataService.DeletePayment(message.EmployerAccountId);
                    await dataService.DeleteEarningDetails(message.EmployerAccountId);

                    logger.Info($"{nameof(CreatePaymentMessageFunction)} finished, Payments created: {paymentCreatedMessage.Count}");
                });
        }

        private static PaymentCreatedMessage CreatePaymentSubstituteData(EmployerPayment payment, IEnumerable<EarningDetails> earningDetails, long? substitutionId)
        {
            var ed = earningDetails.FirstOrDefault(m => m.RequiredPaymentId == payment.PaymentId);
            if (payment == null || ed == null)
                return null;

            Random r = new Random();

            var paymentId = Guid.NewGuid();
            ed.RequiredPaymentId = paymentId;
            return new PaymentCreatedMessage
            {
                Id = paymentId.ToString(),
                EmployerAccountId = substitutionId.Value,
                Ukprn = 1,
                ApprenticeshipId = r.Next(10000, 99999),
                Amount = payment.Amount,
                ProviderName = "Provider Name",
                ApprenticeName = "Apprentice Name",
                CourseName = payment.ApprenticeshipCourseName,
                CourseLevel = payment.ApprenticeshipCourseLevel,
                Uln = 1234567890,
                CourseStartDate = payment.ApprenticeshipCourseStartDate,
                CollectionPeriod = new Application.Payments.Messages.CollectionPeriod { Id = payment.CollectionPeriodId, Year = payment.CollectionPeriodYear, Month = payment.CollectionPeriodMonth },
                EarningDetails = ed,
                FundingSource = payment.FundingSource
            };
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
                FundingSource = payment.FundingSource
            };
        }
    }
}
