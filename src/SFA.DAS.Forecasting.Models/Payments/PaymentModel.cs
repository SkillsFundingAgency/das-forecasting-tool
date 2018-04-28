using System;

namespace SFA.DAS.Forecasting.Models.Payments
{
    public class PaymentModel
    {
        public long Id { get; set; }
        public string ExternalPaymentId { get; set; }
        public long EmployerAccountId { get; set; }
        public long ProviderId { get; set; }
        public long ApprenticeshipId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ReceivedTime { get; set; }
        public long LearnerId { get; set; }
        public NamedCalendarPeriod CollectionPeriod { get; set; }
        public CalendarPeriod DeliveryPeriod { get; set; }
        public FundingSource FundingSource { get; set; }
    }
}
