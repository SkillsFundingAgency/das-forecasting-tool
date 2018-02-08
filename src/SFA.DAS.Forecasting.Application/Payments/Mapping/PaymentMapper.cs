using System;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Domain.Payments.Entities;
using CollectionPeriod = SFA.DAS.Forecasting.Domain.Payments.Entities.CollectionPeriod;

namespace SFA.DAS.Forecasting.Application.Payments.Mapping
{
	public class PaymentMapper
	{
		public Payment MapToPayment(PaymentEvent paymentEvent)
		{
			return new Payment
			{
				Id = paymentEvent.Id,
				EmployerAccountId = paymentEvent.EmployerAccountId,
				ProviderName = paymentEvent.ProviderName,
				ProviderId = paymentEvent.Ukprn,
				LearnerId = paymentEvent.Uln,
				FirstName = paymentEvent.ApprenticeName,
				LastName = paymentEvent.ApprenticeName,
				Amount = paymentEvent.Amount,
				CollectionPeriod = new CollectionPeriod
				{
					Id = paymentEvent.CollectionPeriod.Id,
					Month = paymentEvent.CollectionPeriod.Month,
					Year = paymentEvent.CollectionPeriod.Year
				},
				ApprenticeshipId = paymentEvent.ApprenticeshipId,
				ReceivedTime = DateTime.Now
			};
		}
	}
}
