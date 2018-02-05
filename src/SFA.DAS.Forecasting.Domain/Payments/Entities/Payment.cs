using System;

namespace SFA.DAS.Forecasting.Domain.Payments.Entities
{
	public class Payment
	{
		public string Id { get; set; }

		public string EmployerAccountId { get; set; }

		public long Ukprn { get; set; }

		public long ApprenticeshipId { get; set; }

		public decimal Amount { get; set; }

		public DateTime ReceivedTime { get; set; }

		public long Uln { get; set; }

		public string ProviderName { get; set; }

		public CollectionPeriod CollectionPeriod { get; set; }

		public string FirstName { get; set; }
		public string LastName { get; set; }
	}
}
