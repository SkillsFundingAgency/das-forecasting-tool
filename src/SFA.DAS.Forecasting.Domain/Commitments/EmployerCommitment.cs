using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Commitments.Validation;
using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Domain.Commitments
{
    public class EmployerCommitment
    {
        internal CommitmentModel Commitment { get; private set; }
        private readonly ICommitmentValidator _commitmentValidator;

        public long Id => Commitment.Id;
        public long EmployerAccountId => Commitment.EmployerAccountId;
        public long ApprenticeshipId => Commitment.ApprenticeshipId;
        public long LearnerId => Commitment.LearnerId;
        public DateTime StartDate => Commitment.StartDate;
        public DateTime PlannedEndDate => Commitment.PlannedEndDate;
        public DateTime? ActualEndDate => Commitment.ActualEndDate;
        public decimal CompletionAmount => Commitment.CompletionAmount;
        public decimal MonthlyInstallment => Commitment.MonthlyInstallment;
        public short NumberOfInstallments => Commitment.NumberOfInstallments;
        public long ProviderId => Commitment.ProviderId;
        public string ProviderName => Commitment.ProviderName;
        public string ApprenticeName => Commitment.ApprenticeName;
        public string CourseName => Commitment.CourseName;
        public int? CourseLevel => Commitment.CourseLevel;

        public EmployerCommitment(CommitmentModel commitment, ICommitmentValidator commitmentValidator)
        {
            Commitment = commitment ?? throw new ArgumentNullException(nameof(commitment));
            _commitmentValidator = commitmentValidator ?? throw new ArgumentNullException(nameof(commitmentValidator));
        }

        public bool RegisterCommitment(long learnerId, string apprenticeName, string courseName, int? courseLevel,
            long providerId, string providerName, DateTime startDate, DateTime plannedEndDate,
            DateTime? actualEndDate, decimal monthlyInstallment, decimal completionAmount, short numberOfInstallments)
        {
            //TODO: move into validation class
            if (Commitment.EmployerAccountId <= 0)
                return false;

            if (actualEndDate.HasValue && actualEndDate == DateTime.MinValue)
                actualEndDate = null;
            Commitment.ApprenticeName = apprenticeName;
            Commitment.LearnerId = learnerId;
            Commitment.CourseLevel = courseLevel;
            Commitment.CourseName = courseName;
            Commitment.ProviderId = providerId;
            Commitment.ProviderName = providerName;
            Commitment.StartDate = startDate;
            Commitment.PlannedEndDate = plannedEndDate;
            Commitment.ActualEndDate = actualEndDate;
            Commitment.MonthlyInstallment = monthlyInstallment;
            Commitment.CompletionAmount = completionAmount;
            Commitment.NumberOfInstallments = numberOfInstallments;

            return Commitment.Id > 0 || _commitmentValidator.IsValid(Commitment);
        }
    }
}