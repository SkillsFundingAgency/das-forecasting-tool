using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Domain.Commitments
{
    public partial class EmployerCommitments
    {
        private readonly long _employerAccountId;
        private readonly List<CommitmentModel> _commitments;
        private readonly List<CommitmentModel> _commitmentsAsSender;

        public ReadOnlyCollection<CommitmentModel> Commitments => _commitments.AsReadOnly();


        public EmployerCommitments(
            long employerAccountId,
            List<CommitmentModel> commitments,
            List<CommitmentModel> commitmentsAsSender)
        {
            _employerAccountId = employerAccountId;
            _commitments = commitments ?? throw new ArgumentNullException(nameof(commitments));
            _commitmentsAsSender = commitmentsAsSender ?? new List<CommitmentModel>();
        }

        public virtual TotalCostOfTraining GetTotalCostOfTraining(DateTime date)
        {
            var commitments = _commitments.Where(FilterCommitments(date));
            var commitmentsAsSender = _commitmentsAsSender.Where(FilterCommitments(date));

            return new TotalCostOfTraining
            {
                Value = commitments.Sum(c => c.MonthlyInstallment),
                CommitmentIds = commitments.Select(c => c.Id),
                TotalCostAsSender = commitmentsAsSender.Sum(m => m.MonthlyInstallment)
            };
        }

        public virtual Tuple<decimal, List<long>> GetTotalCompletionPayments(DateTime date)
        {
            var startOfMonth = date.GetStartOfMonth();
            var commitments = _commitments.Where(commitment =>
                   commitment.PlannedEndDate.GetStartOfMonth().AddMonths(1) == startOfMonth)
                .ToList();
            return new Tuple<decimal, List<long>>(commitments.Sum(c => c.CompletionAmount), commitments.Select(c => c.Id).ToList());
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

        private static Func<CommitmentModel, bool> FilterCommitments(DateTime date)
        {
            var startOfMonth = date.GetStartOfMonth();
            return commitment =>
                                commitment.StartDate.GetStartOfMonth() < startOfMonth &&
                                commitment.PlannedEndDate.GetLastPaymentDate().GetStartOfMonth() >= startOfMonth;
        }
    }
}