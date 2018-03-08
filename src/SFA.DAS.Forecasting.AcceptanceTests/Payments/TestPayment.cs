namespace SFA.DAS.Forecasting.AcceptanceTests.Payments
{
    public class TestPayment:TestCommitment
    {
        public string PaymentId { get; set; }
        public int ApprenticeshipId { get; set; }
        public int ProviderId { get; set; }
        public decimal PaymentAmount { get; set; }
    }
}