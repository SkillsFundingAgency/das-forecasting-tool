namespace SFA.DAS.Forecasting.Application.Payments.Messages
{
	/// <summary>
	/// TODO: Temp event definition. this will be replaced by the actual Payment event published by the employer services.
	/// </summary>
	public class PaymentEvent
    {
        public string Id { get; set; }

		public string EmployerAccountId { get; set; }

		public long Ukprn { get; set; }

        public long ApprenticeshipId { get; set; }

	    public decimal Amount { get; set; }

	    public EarningDetails EarningDetails { get; set; }
	    public CollectionPeriod CollectionPeriod { get; set; }

		public int? FrameworkCode { get; set; }
	    public int? PathwayCode { get; set; }
		public int? ProgrammeType { get; set; }
        public long? StandardCode { get; set; }
    }
}