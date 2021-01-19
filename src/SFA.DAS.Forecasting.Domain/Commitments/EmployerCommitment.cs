using System;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Domain.Commitments
{
    public class EmployerCommitment
    {
        internal CommitmentModel Commitment { get; private set; }

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

        public Status? Status => Commitment.Status;

        public EmployerCommitment(CommitmentModel commitment)
        {
            Commitment = commitment ?? throw new ArgumentNullException(nameof(commitment));
        }

        public bool RegisterCommitment(CommitmentModel model)
        {
            if (model.EmployerAccountId <= 0)
            {
                return false;
            }

            if (model.ActualEndDate.HasValue && model.ActualEndDate == DateTime.MinValue && Commitment.Status != Models.Commitments.Status.Completed && Commitment.Status != Models.Commitments.Status.Stopped)
            {
                model.ActualEndDate = null;
            }

            if (Commitment.Id == 0 && model.ActualEndDate != null)
            {
                return false;
            }
            
            if (Commitment.Id != 0
                && (model.ActualEndDate != Commitment.ActualEndDate
                    || model.ApprenticeName != Commitment.ApprenticeName
                    || model.ApprenticeshipId != Commitment.ApprenticeshipId
                    || model.StartDate != Commitment.StartDate
                    || model.PlannedEndDate != Commitment.PlannedEndDate
                    || model.MonthlyInstallment != Commitment.MonthlyInstallment
                    || model.CompletionAmount != Commitment.CompletionAmount
                    || model.NumberOfInstallments != Commitment.NumberOfInstallments
                    || model.HasHadPayment != Commitment.HasHadPayment))
            {
                //don't update the actual end date - as the Actual EndDate updated from the das-forecasting-jobs event handlers
                Commitment.ApprenticeName = model.ApprenticeName;
                Commitment.ApprenticeshipId = model.ApprenticeshipId;
                Commitment.StartDate = model.StartDate;
                Commitment.PlannedEndDate = model.PlannedEndDate;
                Commitment.MonthlyInstallment = model.MonthlyInstallment;
                Commitment.CompletionAmount = model.CompletionAmount;
                Commitment.NumberOfInstallments = model.NumberOfInstallments;
                Commitment.UpdatedDateTime = model.UpdatedDateTime;
                Commitment.HasHadPayment = model.HasHadPayment;
                return true;
            }

            if (Commitment.Id != 0)
            {
                return false;
            }
    
            Commitment.ApprenticeshipId = model.ApprenticeshipId;
            Commitment.ApprenticeName = model.ApprenticeName;
            Commitment.LearnerId = model.LearnerId;
            Commitment.CourseLevel = model.CourseLevel;
            Commitment.CourseName = model.CourseName;
            Commitment.ProviderId = model.ProviderId;
            Commitment.ProviderName = model.ProviderName;
            Commitment.StartDate = model.StartDate;
            Commitment.PlannedEndDate = model.PlannedEndDate;
            Commitment.MonthlyInstallment = model.MonthlyInstallment;
            Commitment.CompletionAmount = model.CompletionAmount;
            Commitment.NumberOfInstallments = model.NumberOfInstallments;
            Commitment.SendingEmployerAccountId = model.SendingEmployerAccountId;
            Commitment.FundingSource = model.FundingSource;
            Commitment.EmployerAccountId = model.EmployerAccountId;
            Commitment.UpdatedDateTime = model.UpdatedDateTime;
            Commitment.HasHadPayment = model.HasHadPayment;
            Commitment.Status = Models.Commitments.Status.LiveOrWaitingToStart; 
            return true;
        }
    }
}