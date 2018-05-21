using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Payments.Services;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Messages.Payments.PreLoad;
using SFA.DAS.Forecasting.Models.Payments;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class CreatePaymentMessageFunction : IFunction
    {
        [FunctionName("CreatePaymentMessageFunction")]
        [return: Queue(QueueNames.RemovePreLoadData)]
        public static PreLoadPaymentMessage Run(
            [QueueTrigger(QueueNames.CreatePaymentMessage)]PreLoadPaymentMessage message,
            [Queue(QueueNames.PaymentValidator)] ICollector<PaymentCreatedMessage> outputQueueMessage,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            return FunctionRunner.Run<CreatePaymentMessageFunction, PreLoadPaymentMessage>(writer, executionContext,
                (container, logger) =>
                {
                    logger.Info($"{nameof(CreatePaymentMessageFunction)} started");

                    var dataService = container.GetInstance<PreLoadPaymentDataService>();
                    var payments = dataService.GetPayments(message.EmployerAccountId).ToList();
                    var earningDetails = dataService.GetEarningDetails(message.EmployerAccountId).ToList();
                    logger.Info($"Got {payments.Count()} payments to match against {earningDetails.Count} earning details for employer '{message.EmployerAccountId}'");
                    List<PaymentCreatedMessage> paymentCreatedMessage;
                    if (message.SubstitutionId != null)
                    {
                        paymentCreatedMessage =
                            payments
                            .Select(p => CreatePaymentSubstituteData(logger, p, earningDetails, message.SubstitutionId.Value))
                            .Where(p => p != null)
                            .ToList();
                    }
                    else
                    {
                        paymentCreatedMessage =
                            payments
                            .Select(p => CreatePayment(logger, p, earningDetails))
                            .Where(p => p != null)
                            .ToList();
                    }

                    foreach (var p in paymentCreatedMessage)
                    {
                        outputQueueMessage.Add(p);
                    }
                    logger.Info($"{nameof(CreatePaymentMessageFunction)} finished, Payments created: {paymentCreatedMessage.Count}");
                    return message;
                });
        }

        private static readonly Dictionary<long, long> Apprenticeships = new Dictionary<long, long>();
        private static readonly object LockObject = new object();

        private static long GetApprenticeshipId(long originalApprenticeshipId)
        {
            lock (LockObject)
            {
                if (Apprenticeships.ContainsKey(originalApprenticeshipId))
                    return Apprenticeships[originalApprenticeshipId];
                var random = new Random(Guid.NewGuid().GetHashCode());
                var id = random.Next(1, 9999999);
                var cnt = 0;
                while (Apprenticeships.ContainsValue(id) && cnt++ < 1000)
                    id = random.Next(1, 9999999);
                Apprenticeships[originalApprenticeshipId] = id;
                return Apprenticeships[originalApprenticeshipId];
            }
        }
        private static PaymentCreatedMessage CreatePaymentSubstituteData(ILog logger, EmployerPayment payment, IEnumerable<EarningDetails> earningDetails, long substitutionId)
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

            var apprenticeshipId = GetApprenticeshipId(payment.ApprenticeshipId);
            logger.Info($"Creating payment event for apprenticeship: {apprenticeshipId}, delivery period: {payment.DeliveryPeriodYear}-{payment.DeliveryPeriodMonth}, collection period: {payment.CollectionPeriodYear}-{payment.CollectionPeriodMonth}");
            earningDetail.RequiredPaymentId = Guid.NewGuid();
            return new PaymentCreatedMessage
            {
                Id = Guid.NewGuid().ToString(),
                EmployerAccountId = substitutionId,
                Ukprn = 1,
                ApprenticeshipId = apprenticeshipId,
                Amount = payment.Amount,
                ProviderName = "Provider Name",
                ApprenticeName = "Apprentice Name",
                CourseName = payment.ApprenticeshipCourseName,
                CourseLevel = payment.ApprenticeshipCourseLevel,
                Uln = 1234567890,
                CourseStartDate = payment.ApprenticeshipCourseStartDate,
                CollectionPeriod = new Application.Payments.Messages.NamedCalendarPeriod { Id = payment.CollectionPeriodId, Year = payment.CollectionPeriodYear, Month = payment.CollectionPeriodMonth },
                DeliveryPeriod = new Application.Payments.Messages.CalendarPeriod { Month = payment.DeliveryPeriodMonth, Year = payment.DeliveryPeriodYear },
                EarningDetails = earningDetail,
                FundingSource = payment.FundingSource
            };
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
                FundingSource = payment.FundingSource
            };
        }
    }
}
