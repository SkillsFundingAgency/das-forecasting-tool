using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Commitments.Validation;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Domain.Commitments
{
    public class EmployerCommitment
    {
        internal CommitmentModel Commitment { get; private set; }
        private readonly ICommitmentValidator _commitmentValidator;

        public long Id => Commitment.Id;
        public long EmployerAccountId => Commitment.EmployerAccountId;
        public long SendingEmployerAccountId => Commitment.SendingEmployerAccountId;
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

        public FundingSource FundingSource => Commitment.FundingSource;

        public EmployerCommitment(CommitmentModel commitment, ICommitmentValidator commitmentValidator)
        {
            Commitment = commitment ?? throw new ArgumentNullException(nameof(commitment));
            _commitmentValidator = commitmentValidator ?? throw new ArgumentNullException(nameof(commitmentValidator));
        }

        public bool RegisterCommitment(CommitmentModel model)
        {
            if (Commitment.EmployerAccountId <= 0)
                return false;

            if (model.ActualEndDate.HasValue && model.ActualEndDate == DateTime.MinValue)
                model.ActualEndDate = null;

            if (Commitment.Id == 0 && model.ActualEndDate != null)
                return false;

            if (Commitment.ApprenticeName == model.ApprenticeName &&
                Commitment.LearnerId == model.LearnerId &&
                Commitment.CourseLevel == model.CourseLevel &&
                Commitment.CourseName == model.CourseName &&
                Commitment.ProviderId == model.ProviderId &&
                Commitment.ProviderName == model.ProviderName &&
                Commitment.StartDate == model.StartDate &&
                Commitment.PlannedEndDate == model.PlannedEndDate &&
                Commitment.MonthlyInstallment == model.MonthlyInstallment &&
                Commitment.CompletionAmount == model.CompletionAmount &&
                Commitment.NumberOfInstallments == model.NumberOfInstallments &&
                Commitment.SendingEmployerAccountId == model.SendingEmployerAccountId)
                return false;  //No need to update the 

            Commitment.ApprenticeName = model.ApprenticeName;
            Commitment.LearnerId = model.LearnerId;
            Commitment.CourseLevel = model.CourseLevel;
            Commitment.CourseName = model.CourseName;
            Commitment.ProviderId = model.ProviderId;
            Commitment.ProviderName = model.ProviderName;
            Commitment.StartDate = model.StartDate;
            Commitment.PlannedEndDate = model.PlannedEndDate;
            Commitment.ActualEndDate = model.ActualEndDate;
            Commitment.MonthlyInstallment = model.MonthlyInstallment;
            Commitment.CompletionAmount = model.CompletionAmount;
            Commitment.NumberOfInstallments = model.NumberOfInstallments;
            Commitment.SendingEmployerAccountId = model.SendingEmployerAccountId;
            Commitment.FundingSource = model.FundingSource;

            // Always returns true. Do we need IsValid?
            return Commitment.Id > 0 || _commitmentValidator.IsValid(Commitment);
        }
    }
}