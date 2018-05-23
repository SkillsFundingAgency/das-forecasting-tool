using SFA.DAS.Forecasting.Models.Payments;
using System;

namespace SFA.DAS.Forecasting.AcceptanceTests.Payments
{
    public class TestPayment:TestCommitment
    {
        public string PaymentId { get; set; }
        public int ApprenticeshipId { get; set; }
        public int ProviderId { get; set; }
        public decimal PaymentAmount { get; set; }
        public CalendarPeriod DeliveryPeriod { get; set; }
    }
}