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
            var commitments = Commitments.Where(FilterCommitments(date));
            var commitmentsAsSender = CommitmentsTransfers.Where(FilterCommitments(date));

            return new TotalCostOfTraining
            {
                Value = commitments.Sum(c => c.MonthlyInstallment),
                CommitmentIds = commitments.Select(c => c.Id),
                TransferCost = commitmentsAsSender.Sum(m => m.MonthlyInstallment)
            };
        }

        public virtual Tuple<decimal, List<long>> GetTotalCompletionPayments(DateTime date)
        {
            var startOfMonth = date.GetStartOfMonth();
            var commitments = Commitments.Where(commitment =>
                   commitment.PlannedEndDate.GetStartOfMonth().AddMonths(1) == startOfMonth)
                .ToList();
            return new Tuple<decimal, List<long>>(commitments.Sum(c => c.CompletionAmount), commitments.Select(c => c.Id).ToList());
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

        private static Func<CommitmentModel, bool> FilterCommitments(DateTime date)
        {
            var startOfMonth = date.GetStartOfMonth();
            return commitment =>
                                commitment.StartDate.GetStartOfMonth() < startOfMonth &&
                                commitment.PlannedEndDate.GetLastPaymentDate().GetStartOfMonth() >= startOfMonth;
        }
    }
}