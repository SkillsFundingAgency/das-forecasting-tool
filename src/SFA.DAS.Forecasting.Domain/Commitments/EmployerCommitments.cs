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

        private IEnumerable<CommitmentModel> CommitmentsReceived => _commitments
                                        .Where(m => m.EmployerAccountId == _employerAccountId)
                                        .Where(m => m.FundingSource == Models.Payments.FundingSource.Levy);

        private IEnumerable<CommitmentModel> CommitmentsTransferReceived => _commitments
                                        .Where(m => m.EmployerAccountId == _employerAccountId)
                                        .Where(m => m.FundingSource == Models.Payments.FundingSource.Transfer);

        private IEnumerable<CommitmentModel> CommitmentsTransferSent => _commitments.Where(m => m.SendingEmployerAccountId == _employerAccountId);


        public EmployerCommitments(
            long employerAccountId,
            List<CommitmentModel> commitments)
        {
            _employerAccountId = employerAccountId;
            _commitments = commitments ?? throw new ArgumentNullException(nameof(commitments));
        }

        public virtual CostOfTraining GetTotalCostOfTraining(DateTime date)
        {
            Func<CommitmentModel, bool> filterCurrent = c =>
                      c.StartDate.GetStartOfMonth() < date.GetStartOfMonth() &&
                      c.PlannedEndDate.GetLastPaymentDate().GetStartOfMonth() >= date.GetStartOfMonth();

            var commitmentsReceived = CommitmentsReceived.Where(filterCurrent);

            return new CostOfTraining
            {
                LevyOut = commitmentsReceived.Sum(c => c.MonthlyInstallment),
                TransferOut = CommitmentsTransferSent.Where(filterCurrent)
                                                     .Sum(m => m.MonthlyInstallment),
                CommitmentIds = commitmentsReceived.Select(c => c.Id),
                TransferIn = CommitmentsTransferReceived.Where(filterCurrent)
                                                        .Sum(m => m.MonthlyInstallment)
            };
        }

        public virtual CompletionPayments GetTotalCompletionPayments(DateTime date)
        {
            Func<CommitmentModel, bool> filterCurrent = c => c.PlannedEndDate.GetStartOfMonth().AddMonths(1) == date.GetStartOfMonth();

            var commitmentsReceived = CommitmentsReceived.Where(filterCurrent);

            return new CompletionPayments
            {
                LevyCompletionPaymentOut = commitmentsReceived.Sum(c => c.CompletionAmount),
                TransferCompletionPaymentOut = CommitmentsTransferSent.Where(filterCurrent)
                                                                      .Sum(m => m.CompletionAmount),
                CommitmentIds = commitmentsReceived.Select(c => c.Id).ToList(),
                TransferCompletionPaymentIn = CommitmentsTransferReceived.Where(filterCurrent)
                                                                         .Sum(m => m.CompletionAmount)
            };
        }

        public DateTime GetEarliestCommitmentStartDate()
        {
            return _commitments.OrderBy(commitment => commitment.StartDate)
                .Select(commitment => commitment.StartDate)
                .FirstOrDefault();
        }
        public DateTime GetLastCommitmentPlannedEndDate()
        {
            return _commitments.OrderByDescending(commitment => commitment.PlannedEndDate)
                .Select(commitment => commitment.PlannedEndDate)
                .FirstOrDefault();
        }
    }
}