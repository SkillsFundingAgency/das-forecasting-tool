using System;

namespace SFA.DAS.Forecasting.Application.Payments.Messages
{
	/// <summary>
	/// TODO: Temp event definition. this will be replaced by the actual Payment event published by the employer services.
	/// </summary>
	public class PaymentEvent
    {
        public string Id { get; set; }
		public long EmployerAccountId { get; set; }
		public long Ukprn { get; set; }
        public long ApprenticeshipId { get; set; }
	    public decimal Amount { get; set; }
        public string ProviderName { get; set; }
        public string ApprenticeName { get; set; }
        public string CourseName { get; set; }
        public int? CourseLevel { get; set; }
	    public long Uln { get; set; }

        public DateTime? CourseStartDate { get; set; }

        public EarningDetails EarningDetails { get; set; }
	    public CollectionPeriod CollectionPeriod { get; set; }
    }


    //public class PaymentCreatedMessage : AccountMessageBase
    //{
    //    public decimal Amount { get; set; }
    //    public string PeriodEnd { get; set; }
    //    public string ProviderName { get; set; }
    //    public string CourseName { get; set; }
    //    public int? CourseLevel { get; set; }
    //    public DateTime? CourseStartDate { get; set; }
    //    public string ApprenticeName { get; set; }
    //    public string ApprenticeNINumber { get; set; }
    //    public string ApprenticeshipVersion { get; set; }
    //    public long ApprenticeshipId { get; set; }
    //    public long Ukprn { get; set; }
    //    public long Uln { get; set; }
    //    public int DeliveryPeriodMonth { get; set; }
    //    public int DeliveryPeriodYear { get; set; }
    //    public string CollectionPeriodId { get; set; }
    //    public int CollectionPeriodMonth { get; set; }
    //    public int CollectionPeriodYear { get; set; }
    //    public DateTime EvidenceSubmittedOn { get; set; }
    //    public FundingSource FundingSource { get; set; }
    //    public TransactionType TransactionType { get; set; }

    //    public PaymentCreatedMessage()
    //        : base(0, string.Empty, string.Empty)
    //    { }

    //    public PaymentCreatedMessage(long accountId, string creatorName, string creatorUserRef)
    //        : base(accountId, creatorName, creatorUserRef)
    //    {
    //    }
    //}
}