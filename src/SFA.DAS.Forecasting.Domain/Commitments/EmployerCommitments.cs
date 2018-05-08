using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Domain.Commitments
{
    public partial class EmployerCommitments
    {
        private readonly long _employerAccountId;
        private readonly IEnumerable<CommitmentModel> _commitments;

        private IEnumerable<CommitmentModel> Commitments => _commitments.Where(m => m.EmployerAccountId == _employerAccountId);
        private IEnumerable<CommitmentModel> CommitmentsTransfers => _commitments.Where(m => m.SendingEmployerAccountId == _employerAccountId);


        public EmployerCommitments(
            long employerAccountId,
            List<CommitmentModel> commitments)
        {
            _employerAccountId = employerAccountId;
            _commitments = commitments ?? throw new ArgumentNullException(nameof(commitments));
        }

        public virtual TotalCostOfTraining GetTotalCostOfTraining(DateTime date)
        {
            Func<CommitmentModel, bool> filterCurrent = c =>
                      c.StartDate.GetStartOfMonth() < date.GetStartOfMonth() &&
                      c.PlannedEndDate.GetLastPaymentDate().GetStartOfMonth() >= date.GetStartOfMonth();

            var commitments = Commitments.Where(filterCurrent);
            var commitmentsAsSender = CommitmentsTransfers.Where(filterCurrent);

            return new TotalCostOfTraining
            {
                Value = commitments.Sum(c => c.MonthlyInstallment),
                CommitmentIds = commitments.Select(c => c.Id),
                TransferCost = commitmentsAsSender.Sum(m => m.MonthlyInstallment)
            };
        }

        public virtual TotalCompletionPayments GetTotalCompletionPayments(DateTime date)
        {
            Func<CommitmentModel, bool> filterCurrent = c => c.PlannedEndDate.GetStartOfMonth().AddMonths(1) == date.GetStartOfMonth();

            var commitments = Commitments.Where(filterCurrent);
            var commitmentsAsSender = CommitmentsTransfers.Where(filterCurrent);

            return new TotalCompletionPayments
            {
                Value = commitments.Sum(c => c.CompletionAmount),
                CommitmentIds = commitments.Select(c => c.Id).ToList(),
                TransferOut = commitmentsAsSender.Sum(m => m.CompletionAmount)
            };

        }

        public DateTime GetEarliestCommitmentStartDate()
        {
            return Commitments.OrderBy(commitment => commitment.StartDate)
                .Select(commitment => commitment.StartDate)
                .FirstOrDefault();
        }
        public DateTime GetLastCommitmentPlannedEndDate()
        {
            return Commitments.OrderByDescending(commitment => commitment.PlannedEndDate)
                .Select(commitment => commitment.PlannedEndDate)
                .FirstOrDefault();
        }
    }
}