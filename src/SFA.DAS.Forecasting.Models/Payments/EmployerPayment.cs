using System;

namespace SFA.DAS.Forecasting.Models.Payments
{
    public class EmployerPayment
    {
        public Guid PaymentId { get; set; }
        public long Ukprn;
        public long Uln;
        public long AccountId;
        public long ApprenticeshipId;
        public string CollectionPeriodId;
        public int CollectionPeriodMonth;
        public int CollectionPeriodYear;
        public decimal Amount;
        public long PaymentMetaDataId;
        public string ProviderName;
        public int StandardCode;
        public int FrameworkCode;
        public int ProgrammeType;
        public int PathwayCode;
        public string PathwayName;
        public string ApprenticeshipCourseName;
        public DateTime? ApprenticeshipCourseStartDate;
        public int ApprenticeshipCourseLevel;
        public string ApprenticeName;
    }
}