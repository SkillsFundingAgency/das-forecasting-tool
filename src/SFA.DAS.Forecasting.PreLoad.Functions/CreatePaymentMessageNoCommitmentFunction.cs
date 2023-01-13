using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Payments.Messages.PreLoad;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class CreatePaymentMessageNoCommitmentFunction 
    {
	    private readonly IEmployerDatabaseService _employerDatabaseService;

	    public CreatePaymentMessageNoCommitmentFunction(IEmployerDatabaseService employerDatabaseService)
	    {
		    _employerDatabaseService = employerDatabaseService;
	    }
        [FunctionName("CreatePaymentMessageNoCommitmentFunction")]
        [return: Queue(QueueNames.RemovePreLoadDataNoCommitment)]
        public async Task<PreLoadPaymentMessage> Run(
            [QueueTrigger(QueueNames.CreatePaymentMessageNoCommitment)]PreLoadPaymentMessage message,
            [Queue(QueueNames.PaymentValidatorNoCommitment)] ICollector<PaymentCreatedMessage> noCommitmentOutputQueueMessage,
			ILogger logger)
        {
            
            logger.LogInformation($"{nameof(CreatePaymentMessageFunction)} started");

            var payments = await _employerDatabaseService.GetPastEmployerPayments(message.EmployerAccountId,message.PeriodYear, message.PeriodMonth);

            logger.LogInformation($"Got {payments.Count} payments for employer '{message.EmployerAccountId}'");

			
		    var paymentNoCommitmentCreatedMessage =
		        payments
                    .Select(p => CreatePayment(logger, p))
				    .Where(p => p != null)
				    .ToList();
	    

	        foreach (var p in paymentNoCommitmentCreatedMessage)
	        {
		        noCommitmentOutputQueueMessage.Add(p);
	        }

	        logger.LogInformation($"{nameof(CreatePaymentMessageNoCommitmentFunction)} finished, Past payments created: {paymentNoCommitmentCreatedMessage.Count}");

			return message;
        }
        
        private PaymentCreatedMessage CreatePayment(ILogger logger, EmployerPayment payment)
        {
            if (payment == null)
            {
                logger.LogWarning("No payment passed to PaymentCreatedMessage");
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
