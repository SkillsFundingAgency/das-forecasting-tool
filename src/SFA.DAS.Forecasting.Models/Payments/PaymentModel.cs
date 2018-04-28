using System;

namespace SFA.DAS.Forecasting.Models.Payments
{
    public class PaymentModel
    {
        public long Id { get; set; } // Id (Primary key)
        public string ExternalPaymentId { get; set; } // ExternalPaymentId (length: 100)
        public long EmployerAccountId { get; set; } // EmployerAccountId
        public long ProviderId { get; set; } // ProviderId
        public long ApprenticeshipId { get; set; } // ApprenticeshipId
        public decimal Amount { get; set; } // Amount
        public DateTime ReceivedTime { get; set; } // ReceivedTime
        public long LearnerId { get; set; } // LearnerId
        public CalendarPeriod CollectionPeriod { get; set; }
        public CalendarPeriod DeliveryPeriod { get; set; }
        //public int CollectionPeriodMonth { get; set; } // CollectionPeriodMonth
        //public int CollectionPeriodYear { get; set; } // CollectionPeriodYear
        //public int DeliveryPeriodMonth { get; set; } // DeliveryPeriodMonth
        //public int DeliveryPeriodYear { get; set; } // DeliveryPeriodYear
        public FundingSource FundingSource { get; set; } // FundingSource

        public PaymentModel()
        {
            CollectionPeriod = new CalendarPeriod();
            DeliveryPeriod = new CalendarPeriod();
        }
    }
}
