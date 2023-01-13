using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Payments.Messages.PreLoad;
using SFA.DAS.Forecasting.Application.Payments.Services;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class CreatePaymentMessageFunction
    {
        private readonly IPreLoadPaymentDataService _preLoadPaymentDataService;
        private readonly IConfiguration _configuration;

        public CreatePaymentMessageFunction(IPreLoadPaymentDataService preLoadPaymentDataService, IConfiguration configuration)
        {
            _preLoadPaymentDataService = preLoadPaymentDataService;
            _configuration = configuration;
        }
        [FunctionName("CreatePaymentMessageFunction")]
        [return: Queue(QueueNames.RemovePreLoadData)]
        public PreLoadPaymentMessage Run(
            [QueueTrigger(QueueNames.CreatePaymentMessage)]PreLoadPaymentMessage message,
            [Queue(QueueNames.PaymentValidator)] ICollector<PaymentCreatedMessage> outputQueueMessage,
			ILogger logger)
        {
            
            logger.LogInformation($"{nameof(CreatePaymentMessageFunction)} started");

            
            var payments = _preLoadPaymentDataService.GetPayments(message.EmployerAccountId).ToList();
            var earningDetails = _preLoadPaymentDataService.GetEarningDetails(message.EmployerAccountId).ToList();
            logger.LogInformation($"Got {payments.Count()} payments to match against {earningDetails.Count} earning details for employer '{message.EmployerAccountId}'");
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

	        logger.LogInformation($"{nameof(CreatePaymentMessageFunction)} finished, Payments created: {paymentCreatedMessage.Count}");

			return message;
        }

        private static readonly Dictionary<long, long> Apprenticeships = new Dictionary<long, long>();
        private static readonly Dictionary<long, long> Employers = new Dictionary<long, long>();
        private static readonly object LockObject = new object();

        private long GetApprenticeshipId(long originalApprenticeshipId)
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

        private PaymentCreatedMessage CreatePaymentSubstituteData(ILogger logger, EmployerPayment payment, IEnumerable<EarningDetails> earningDetails, long substitutionId)
        {
            if (payment == null)
            {
                logger.LogWarning("No payment passed to CreatePaymentSubstituteData");
                return null;
            }

            var earningDetail = earningDetails.FirstOrDefault(ed => Guid.TryParse(ed.PaymentId, out Guid paymentGuid) && paymentGuid == payment.PaymentId);
            if (earningDetail == null)
            {
                logger.LogWarning($"No earning details found for payment: {payment.PaymentId}, apprenticeship: {payment.ApprenticeshipId}");
                return null;
            }

            var apprenticeshipId = GetApprenticeshipId(payment.ApprenticeshipId);
            logger.LogInformation($"Creating payment event for apprenticeship: {apprenticeshipId}, delivery period: {payment.DeliveryPeriodYear}-{payment.DeliveryPeriodMonth}, collection period: {payment.CollectionPeriodYear}-{payment.CollectionPeriodMonth}");
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
                Uln = new Random(Guid.NewGuid().GetHashCode()).Next(10000, 9999999),
                CourseStartDate = payment.ApprenticeshipCourseStartDate,
                CollectionPeriod = new Application.Payments.Messages.NamedCalendarPeriod { Id = payment.CollectionPeriodId, Year = payment.CollectionPeriodYear, Month = payment.CollectionPeriodMonth },
                DeliveryPeriod = new Application.Payments.Messages.CalendarPeriod { Month = payment.DeliveryPeriodMonth, Year = payment.DeliveryPeriodYear },
                EarningDetails = earningDetail,
                FundingSource = payment.FundingSource,
                SendingEmployerAccountId = payment.SenderAccountId.HasValue ? 54321 : substitutionId //TODO: need to generate or pass in a valid substitute sender account for transfers
            };
        }

        private PaymentCreatedMessage CreatePayment(ILogger logger, EmployerPayment payment, IEnumerable<EarningDetails> earningDetails)
        {
            if (payment == null)
            {
                logger.LogWarning("No payment passed to CreatePaymentSubstituteData");
                return null;
            }
            var earningDetail = earningDetails.FirstOrDefault(ed => Guid.TryParse(ed.PaymentId, out Guid paymentGuid) && paymentGuid == payment.PaymentId);

            if (_configuration["EnvironmentName"] == "LOCAL" && earningDetail == null)
            {
                var random = new Random();
                earningDetail = earningDetails.Skip(random.Next(0, earningDetails.Count() - 1)).FirstOrDefault();
            }
            
            if (earningDetail == null)
            {
                logger.LogWarning($"No earning details found for payment: {payment.PaymentId}, apprenticeship: {payment.ApprenticeshipId}");
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
