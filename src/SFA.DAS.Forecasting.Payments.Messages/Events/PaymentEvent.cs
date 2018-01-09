namespace SFA.DAS.Forecasting.Payments.Messages.Events
{
    public class PaymentEvent
    {
        public string Id { get; set; }
        public string EmployerAccountId { get; set; }
        public long Ukprn { get; set; }

        public long? ApprenticeshipId { get; set; }
        public long Uln { get; set; }

        public int? ProgrammeType { get; set; }
        public int? FrameworkCode { get; set; }

        public long? StandardCode { get; set; }
        public decimal Amount { get; set; }
        public int? PathwayCode { get; set; }
    }
}