using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Domain.Commitments
{
    public partial class EmployerCommitments
    {
        public long EmployerAccountId { get; private set; }
        private readonly IList<CommitmentModel> _commitments;

        private readonly ReadOnlyCollection<CommitmentModel> _levyFundedCommitments;
        private readonly ReadOnlyCollection<CommitmentModel> _receivingEmployerTransferCommitments;
        private readonly ReadOnlyCollection<CommitmentModel> _sendingEmployerTransferCommitments;

        public EmployerCommitments(
            long employerAccountId,
            List<CommitmentModel> commitments)
        {
            EmployerAccountId = employerAccountId;
            _commitments = commitments ?? throw new ArgumentNullException(nameof(commitments));

            _levyFundedCommitments = _commitments
                .Where(m => m.EmployerAccountId == EmployerAccountId)
                .Where(m => m.FundingSource == FundingSource.Levy)
                .ToList()
                .AsReadOnly();

            _receivingEmployerTransferCommitments = _commitments
                .Where(m => m.EmployerAccountId == EmployerAccountId)
                .Where(m => m.FundingSource == FundingSource.Transfer)
                .ToList()
                .AsReadOnly();

            _sendingEmployerTransferCommitments = _commitments
                .Where(m => m.SendingEmployerAccountId == EmployerAccountId)
                .Where(m => m.FundingSource == FundingSource.Transfer)
                .ToList()
                .AsReadOnly();
        }

        public virtual CostOfTraining GetTotalCostOfTraining(DateTime date)
        {
            bool FilterCurrent(CommitmentModel c) =>
                      c.StartDate.GetStartOfMonth() < date.GetStartOfMonth() &&
                      c.PlannedEndDate.GetLastPaymentDate().GetStartOfMonth() >= date.GetStartOfMonth();

            var levyFundedCommitments = _levyFundedCommitments.Where(FilterCurrent).ToList();
            var sendingEmployerCommitments = _sendingEmployerTransferCommitments.Where(FilterCurrent).ToList();
            var receivingEmployerCommitments = _receivingEmployerTransferCommitments.Where(FilterCurrent).ToList();

            var includedCommitments = new List<CommitmentModel>();
            includedCommitments.AddRange(levyFundedCommitments);
            includedCommitments.AddRange(sendingEmployerCommitments);
            includedCommitments.AddRange(receivingEmployerCommitments);

            return new CostOfTraining
            {
                LevyFunded = levyFundedCommitments.Sum(c => c.MonthlyInstallment),
                TransferIn = receivingEmployerCommitments.Sum(m => m.MonthlyInstallment),
                TransferOut = sendingEmployerCommitments.Sum(m => m.MonthlyInstallment) + receivingEmployerCommitments.Sum(c => c.MonthlyInstallment),
                CommitmentIds = includedCommitments.Select(c => c.Id).ToList()
            };
        }

        public virtual CompletionPayments GetTotalCompletionPayments(DateTime date)
        {
            bool FilterCurrent(CommitmentModel c) => c.PlannedEndDate.GetStartOfMonth().AddMonths(1) == date.GetStartOfMonth();

            var levyFundedCommitments = _levyFundedCommitments.Where(FilterCurrent).ToList();
            var sendingEmployerCommitments = _sendingEmployerTransferCommitments.Where(FilterCurrent).ToList();
            var receivingEmployerCommitments = _receivingEmployerTransferCommitments.Where(FilterCurrent).ToList();

            var includedCommitments = new List<CommitmentModel>();
            includedCommitments.AddRange(levyFundedCommitments);
            includedCommitments.AddRange(sendingEmployerCommitments);
            includedCommitments.AddRange(receivingEmployerCommitments);

            return new CompletionPayments
            {
                LevyFundedCompletionPayment = levyFundedCommitments.Sum(c => c.CompletionAmount),
                TransferInCompletionPayment = receivingEmployerCommitments.Sum(m => m.CompletionAmount),
                TransferOutCompletionPayment = sendingEmployerCommitments.Sum(m => m.CompletionAmount) + receivingEmployerCommitments.Sum(m => m.CompletionAmount),
                CommitmentIds = includedCommitments.Select(c => c.Id).ToList()
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

        public virtual decimal GetUnallocatedCompletionAmount()
        {
            return _commitments
                .Where(commitment => commitment.PlannedEndDate < DateTime.Today)
                .Sum(commitment => commitment.CompletionAmount);
        }
        public bool Any()
        {
            return _commitments.Any();
        }
    }
}