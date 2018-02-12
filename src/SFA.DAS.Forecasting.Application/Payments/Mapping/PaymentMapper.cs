using System;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Application.Payments.Mapping
{
	public class PaymentMapper
	{
		public Payment MapToPayment(PaymentCreatedMessage paymentCreatedMessage)
		{
			return new Payment
			{
				ExternalPaymentId = paymentCreatedMessage.Id,
				EmployerAccountId = paymentCreatedMessage.EmployerAccountId,
				ProviderId = paymentCreatedMessage.Ukprn,
				LearnerId = paymentCreatedMessage.Uln,
				Amount = paymentCreatedMessage.Amount,
				CollectionPeriod = new Models.Payments.CollectionPeriod
                {
					Id = paymentCreatedMessage.CollectionPeriod.Id,
					Month = paymentCreatedMessage.CollectionPeriod.Month,
					Year = paymentCreatedMessage.CollectionPeriod.Year
				},
				ApprenticeshipId = paymentCreatedMessage.ApprenticeshipId,
				ReceivedTime = DateTime.Now
			};
		}
	}
}
