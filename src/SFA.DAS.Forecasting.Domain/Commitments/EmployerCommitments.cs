using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Domain.Commitments
{
    public class EmployerCommitments
    {
        private readonly long _employerAccountId;
        private readonly List<CommitmentModel> _commitments;
        public ReadOnlyCollection<CommitmentModel> Commitments => _commitments.AsReadOnly();


        public EmployerCommitments(
            long employerAccountId,
            List<CommitmentModel> commitments)
        {
            _employerAccountId = employerAccountId;
            _commitments = commitments ?? throw new ArgumentNullException(nameof(commitments));
        }

        public virtual Tuple<decimal, List<long>> GetTotalCostOfTraining(DateTime date)
        {
            var startOfMonth = date.GetStartOfMonth();
            var commitments = _commitments.Where(commitment =>
                    commitment.StartDate.GetStartOfMonth() < startOfMonth &&
                    commitment.PlannedEndDate.GetLastPaymentDate().GetStartOfMonth() >= startOfMonth)
                .ToList();
            return new Tuple<decimal, List<long>>(commitments.Sum(c => c.MonthlyInstallment), commitments.Select(c => c.Id).ToList());
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
    }
}