using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Payments;
using SFA.DAS.Forecasting.Models.Projections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SFA.DAS.Forecasting.Domain.Commitments
{
    public partial class EmployerCommitments
    {
        public long EmployerAccountId { get; private set; }
        private readonly EmployerCommitmentsModel _employerCommitmentsModel;

        private readonly ReadOnlyCollection<CommitmentModel> _levyFundedCommitments;
        private readonly ReadOnlyCollection<CommitmentModel> _receivingEmployerTransferCommitments;
        private readonly ReadOnlyCollection<CommitmentModel> _sendingEmployerTransferCommitments;
        private readonly ReadOnlyCollection<CommitmentModel> _coInvestmentCommitments;

        public EmployerCommitments(
            long employerAccountId,
            EmployerCommitmentsModel commitments)
        {
            EmployerAccountId = employerAccountId;
            _employerCommitmentsModel = commitments ?? throw new ArgumentNullException(nameof(commitments));

            _levyFundedCommitments = _employerCommitmentsModel.LevyFundedCommitments.AsReadOnly();

            _receivingEmployerTransferCommitments = _employerCommitmentsModel.ReceivingEmployerTransferCommitments.AsReadOnly();

            _sendingEmployerTransferCommitments = _employerCommitmentsModel.SendingEmployerTransferCommitments.AsReadOnly();

            _coInvestmentCommitments = _employerCommitmentsModel.CoInvestmentCommitments.AsReadOnly();
        }

        public virtual CostOfTraining GetTotalCostOfTraining(DateTime date, bool isVirtual = false)
        {
            bool FilterCurrent(CommitmentModel c) =>
                      c.StartDate.GetStartOfMonth() < date.GetStartOfMonth() &&
                      c.PlannedEndDate.GetLastPaymentDate().GetStartOfMonth() >= date.GetStartOfMonth() &&
                      (!isVirtual || c.FundingSource == FundingSource.Transfer || c.FundingSource == FundingSource.ApprovedPledgeApplication || c.FundingSource == FundingSource.AcceptedPledgeApplication);

            var levyFundedCommitments = _levyFundedCommitments.Where(FilterCurrent).ToList();
            var sendingEmployerCommitments = _sendingEmployerTransferCommitments.Where(FilterCurrent).ToList();
            var receivingEmployerCommitments = _receivingEmployerTransferCommitments.Where(FilterCurrent).ToList();
            var coInvestmentCommitments = _coInvestmentCommitments.Where(FilterCurrent).ToList();

            var includedCommitments = new List<CommitmentModel>();
            includedCommitments.AddRange(levyFundedCommitments);
            includedCommitments.AddRange(sendingEmployerCommitments);
            includedCommitments.AddRange(receivingEmployerCommitments);
            includedCommitments.AddRange(coInvestmentCommitments);

            return new CostOfTraining
            {
                LevyFunded = levyFundedCommitments.Sum(c => c.MonthlyInstallment) + coInvestmentCommitments.Sum(m => m.MonthlyInstallment),
                TransferIn = receivingEmployerCommitments.Sum(m => m.MonthlyInstallment),
                TransferOut = sendingEmployerCommitments.Sum(m => m.MonthlyInstallment) + receivingEmployerCommitments.Sum(c => c.MonthlyInstallment),
                CommitmentIds = includedCommitments.Select(c => c.Id).ToList(),
                ApprovedPledgeApplicationCost = 0.1m,
                AcceptedPledgeApplicationCost = 0.2m,
                PledgeOriginatedCommitmentCost = 0.3m
                //ApprovedPledgeApplicationCost = sendingEmployerCommitments.Where(x => x.FundingSource == FundingSource.ApprovedPledgeApplication).Sum(m => m.MonthlyInstallment),
                //AcceptedPledgeApplicationCost = sendingEmployerCommitments.Where(x => x.FundingSource == FundingSource.AcceptedPledgeApplication).Sum(m => m.MonthlyInstallment),
                //PledgeOriginatedCommitmentCost = sendingEmployerCommitments.Where(x => x.PledgeApplicationId.HasValue && x.FundingSource == FundingSource.Transfer).Sum(m => m.MonthlyInstallment) +
                                                 //receivingEmployerCommitments.Where(x => x.PledgeApplicationId.HasValue && x.FundingSource == FundingSource.Transfer).Sum(c => c.MonthlyInstallment)
            };
        }

        public virtual CompletionPayments GetTotalCompletionPayments(DateTime date, bool isVirtual = false)
        {
            bool FilterCurrent(CommitmentModel c) =>
                c.PlannedEndDate.GetStartOfMonth().AddMonths(1) == date.GetStartOfMonth() && (!isVirtual || c.FundingSource == FundingSource.Transfer );

            var levyFundedCommitments = _levyFundedCommitments.Where(FilterCurrent).ToList();
            var sendingEmployerCommitments = _sendingEmployerTransferCommitments.Where(FilterCurrent).ToList();
            var receivingEmployerCommitments = _receivingEmployerTransferCommitments.Where(FilterCurrent).ToList();
            var coInvestmentCommitments = _coInvestmentCommitments.Where(FilterCurrent).ToList();

            var includedCommitments = new List<CommitmentModel>();
            includedCommitments.AddRange(levyFundedCommitments);
            includedCommitments.AddRange(sendingEmployerCommitments);
            includedCommitments.AddRange(receivingEmployerCommitments);
            includedCommitments.AddRange(coInvestmentCommitments);

            return new CompletionPayments
            {
                LevyFundedCompletionPayment = levyFundedCommitments.Sum(c => c.CompletionAmount) + coInvestmentCommitments.Sum(m => m.CompletionAmount),
                TransferInCompletionPayment = receivingEmployerCommitments.Sum(m => m.CompletionAmount),
                TransferOutCompletionPayment = sendingEmployerCommitments.Sum(m => m.CompletionAmount) + receivingEmployerCommitments.Sum(m => m.CompletionAmount),
                CommitmentIds = includedCommitments.Select(c => c.Id).ToList()
            };
        }

        public DateTime GetLastCommitmentPlannedEndDate()
        {
            //TODO rewrite this
            var dateList = new List<DateTime>
            {
                _employerCommitmentsModel.LevyFundedCommitments
                    .OrderByDescending(commitment => commitment.PlannedEndDate)
                    .Select(commitment => commitment.PlannedEndDate)
                    .FirstOrDefault(),
                _employerCommitmentsModel.ReceivingEmployerTransferCommitments
                    .OrderByDescending(commitment => commitment.PlannedEndDate)
                    .Select(commitment => commitment.PlannedEndDate)
                    .FirstOrDefault(),
                _employerCommitmentsModel.SendingEmployerTransferCommitments
                    .OrderByDescending(commitment => commitment.PlannedEndDate)
                    .Select(commitment => commitment.PlannedEndDate)
                    .FirstOrDefault()
            };

            return dateList.OrderByDescending(c => c.Date).FirstOrDefault();
        }

        public virtual decimal GetUnallocatedCompletionAmount(bool includeUnpaidCompletionPayments = false)
        {
            bool Filter(CommitmentModel commitment) => includeUnpaidCompletionPayments ? GetFilterUnpaid(commitment) : GetFilter(commitment);

            var levyUnallocatedCompletions = _levyFundedCommitments
                .Where(Filter)
                .Sum(commitment => commitment.CompletionAmount);
            var senderUnallocatedCompletions = _sendingEmployerTransferCommitments
                .Where(Filter)
                .Sum(commitment => commitment.CompletionAmount);
            return levyUnallocatedCompletions + senderUnallocatedCompletions;
        }

        private static bool GetFilter(CommitmentModel commitment)
        {
            var limitEndDate = DateTime.Today.GetStartOfMonth();
            return commitment.PlannedEndDate.GetStartOfMonth().AddMonths(1) < limitEndDate;
        }

        private static bool GetFilterUnpaid(CommitmentModel commitment)
        {
            var limitEndDate = DateTime.Today.GetStartOfMonth();
            return  commitment.ActualEndDate == null
                   && commitment.HasHadPayment
                   && commitment.PlannedEndDate.GetStartOfMonth().AddMonths(1) == limitEndDate;
        }

        public bool Any()
        {
            return _employerCommitmentsModel.LevyFundedCommitments.Any()
                || _employerCommitmentsModel.ReceivingEmployerTransferCommitments.Any()
                || _employerCommitmentsModel.SendingEmployerTransferCommitments.Any();
        }

        public bool IsSendingEmployer()
        {
            return _sendingEmployerTransferCommitments.Any();
        }

    }
}