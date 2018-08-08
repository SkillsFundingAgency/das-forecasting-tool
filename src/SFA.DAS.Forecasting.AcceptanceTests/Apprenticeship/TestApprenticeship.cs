using System;
using System.Text.RegularExpressions;

namespace SFA.DAS.Forecasting.AcceptanceTests.Apprenticeship
{
    public class TestApprenticeship
    {
        public long? TransferSenderId { get; set; } = null;
        public long EmployerAccountId { get; set; }
        public long Id { get; set; }
        public string FirstName { get; set; }
        public DateTime? StartDate => GetDate(StartDateString);

        private DateTime? GetDate(string dateString)
        {
            var match = Regex.Match(dateString, "[0-9]*");
            var yearsAgo = int.Parse(match.Value);
            return DateTime.Today.AddYears(-yearsAgo);
        }

        public string StartDateString { get; set; }
        public DateTime? EndDate => GetDate(EndDateString);
        public string EndDateString { get; set; }
        public decimal? Cost { get; set; }

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Active;
        public DateTime? StopDate { get; set; } = null;
        public DateTime? PauseDate { get; set; } = null;
        public long ProviderId { get; set; } = 888;
        public string TrainingCode { get; set; } = "107";
        public string TrainingName { get; set; } = "Embedded electronic systems design and development engineer";
        public TrainingType TrainingType { get; set; } = TrainingType.Standard;
        public string ApprenticeshipName { get; } = "Training name";
        public string LastName { get; set; } = "Mock Last Name";
        public string NINumber { get; set; } = "1234567890";
        public string ULN => "100009" + Id.ToString();

        public AgreementStatus AgreementStatus { get; set; }
        public string EmployerRef { get; set; }
        public string ProviderRef { get; set; }
        public bool CanBeApproved { get; set; }
        public Originator? PendingUpdateOriginator { get; set; }
        public string ProviderName { get; set; }
        public string LegalEntityId { get; set; }
        public string LegalEntityName { get; set; }
        public bool DataLockPrice { get; set; } = false;
        public bool DataLockPriceTriaged { get; set; } = false;
        public bool DataLockCourse { get; set; } = false;
        public bool DataLockCourseTriaged { get; set; } = false;
        public bool DataLockCourseChangeTriaged { get; set; } = false;
        public bool DataLockTriagedAsRestart { get; set; } = false;
        public bool HasHadDataLockSuccess { get; set; } = false;
        public long CommitmentId { get; set; }
        public string Reference { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string EndpointAssessorName { get; set; }
    }

    public enum AgreementStatus : short
    {
        NotAgreed = 0,
        EmployerAgreed = 1,
        ProviderAgreed = 2,
        BothAgreed = 3
    }

    public enum PaymentStatus : short
    {
        PendingApproval = 0,
        Active = 1,
        Paused = 2,
        Withdrawn = 3,
        Completed = 4,
        Deleted = 5
    }

    public enum TrainingType
    {
        Standard = 0,
        Framework = 1
    }

    public enum Originator
    {
        Employer = 0,
        Provider = 1
    }
}
