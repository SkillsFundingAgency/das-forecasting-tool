//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace SFA.DAS.Forecasting.AcceptanceTests.EmployerApiStub.Models
//{
//    public class PageOfResults<T>
//    {
//        public int PageNumber { get; set; }
//        public int TotalNumberOfPages { get; set; }
//        public T[] Items { get; set; }
//    }

//    public class DeliveryPeriod
//    {
//        public int Month { get; set; }
//        public int Year { get; set; }
//    }

//    public class CollectionPeriod
//    {
//        public string Id { get; set; }
//        public int Month { get; set; }
//        public int Year { get; set; }
//    }

//    public class EarningDetail
//    {
//        public string RequiredPaymentId { get; set; }
//        public DateTime StartDate { get; set; }
//        public DateTime PlannedEndDate { get; set; }
//        public DateTime ActualEndDate { get; set; }
//        public int CompletionStatus { get; set; }
//        public int CompletionAmount { get; set; }
//        public int MonthlyInstallment { get; set; }
//        public int TotalInstallments { get; set; }
//    }

//    public class Payment
//    {
//        public string Id { get; set; }

//        public long Ukprn { get; set; }
//        public long Uln { get; set; }
//        public string EmployerAccountId { get; set; }
//        public long? ApprenticeshipId { get; set; }

//        public CalendarPeriod DeliveryPeriod { get; set; }
//        public NamedCalendarPeriod CollectionPeriod { get; set; }

//        public DateTime EvidenceSubmittedOn { get; set; }
//        public string EmployerAccountVersion { get; set; }
//        public string ApprenticeshipVersion { get; set; }

//        public FundingSource FundingSource { get; set; }
//        public TransactionType TransactionType { get; set; }
//        public decimal Amount { get; set; }

//        public long? StandardCode { get; set; }
//        public int? FrameworkCode { get; set; }
//        public int? ProgrammeType { get; set; }
//        public int? PathwayCode { get; set; }

//        public ContractType ContractType { get; set; }
//        public List<Earning> EarningDetails { get; set; }
//    }

//    public class CalendarPeriod
//    {
//        public int Month { get; set; }
//        public int Year { get; set; }
//    }

//    public class NamedCalendarPeriod : CalendarPeriod
//    {
//        public string Id { get; set; }
//    }

//    public enum FundingSource
//    {
//        Levy = 1,
//        CoInvestedSfa = 2,
//        CoInvestedEmployer = 3,
//        FullyFundedSfa = 4
//    }

//    public enum TransactionType
//    {
//        Learning = 1,
//        Completion = 2,
//        Balancing = 3,
//        First16To18EmployerIncentive = 4,
//        First16To18ProviderIncentive = 5,
//        Second16To18EmployerIncentive = 6,
//        Second16To18ProviderIncentive = 7,
//        OnProgramme16To18FrameworkUplift = 8,
//        Completion16To18FrameworkUplift = 9,
//        Balancing16To18FrameworkUplift = 10,
//        FirstDisadvantagePayment = 11,
//        SecondDisadvantagePayment = 12,
//        OnProgrammeMathsAndEnglish = 13,
//        BalancingMathsAndEnglish = 14,
//        LearningSupport = 15
//    }
//}
