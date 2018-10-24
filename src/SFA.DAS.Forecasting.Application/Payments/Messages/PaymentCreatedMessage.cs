using System;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Messages.Projections;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Forecasting.Application.Payments.Messages
{
    /// <summary>
    /// TODO: Temp event definition. this will be replaced by the actual Payment event published by the employer services.
    /// </summary>
    public class PaymentCreatedMessage
    {
        public string Id { get; set; }
		public long EmployerAccountId { get; set; }
        public long SendingEmployerAccountId { get; set; }
        public long Ukprn { get; set; }
        public long ApprenticeshipId { get; set; }
	    public decimal Amount { get; set; }
        public string ProviderName { get; set; }
        public string ApprenticeName { get; set; }
        public string CourseName { get; set; }
        public int? CourseLevel { get; set; }
	    public long Uln { get; set; }
        public FundingSource FundingSource { get; set; }

        public DateTime? CourseStartDate { get; set; }

	    [JsonConverter(typeof(SingleValueArrayConverter<EarningDetails>))]
		public EarningDetails EarningDetails { get; set; }
	    public NamedCalendarPeriod CollectionPeriod { get; set; }
        public CalendarPeriod DeliveryPeriod { get; set; }
        public ProjectionSource ProjectionSource { get; set; }
    }
}