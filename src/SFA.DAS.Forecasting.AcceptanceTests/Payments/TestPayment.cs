using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.AcceptanceTests.Payments
{
    public class TestPayment:TestCommitment
    {
        public string PaymentId { get; set; }
        public int ApprenticeshipId { get; set; }
        public int ProviderId { get; set; }
        public decimal PaymentAmount { get; set; }
        public CalendarPeriod DeliveryPeriod { get; set; }
        public long? SendingEmployerAccountId { get; set; }
    }
}