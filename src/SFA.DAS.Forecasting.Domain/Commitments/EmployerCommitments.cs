using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Domain.Commitments.Validation;
using SFA.DAS.Forecasting.Domain.Events;
using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Domain.Commitments
{
    public class EmployerCommitments
    {
        private readonly long _employerAccountId;
        private readonly List<Commitment> _commitments;
        private readonly IEventPublisher _eventPublisher;
        private readonly CommitmentValidator _commitmentValidator;
        public ReadOnlyCollection<Commitment> Commitments => _commitments.AsReadOnly();


        public EmployerCommitments(
            long employerAccountId, 
            List<Commitment> commitments, 
            IEventPublisher eventPublisher, 
            CommitmentValidator commitmentValidator)
        {
            _employerAccountId = employerAccountId;
            _commitments = commitments ?? throw new ArgumentNullException(nameof(commitments));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
            _commitmentValidator = commitmentValidator;
        }

        public bool AddCommitment(long apprenticeshipId, long learnerId, DateTime startDate, DateTime plannedEndDate,
            DateTime? actualEndDate, decimal monthlyInstallment, decimal completionAmount, short numberOfInstallments)
        {
            var commitment = new Commitment
            {
                EmployerAccountId = _employerAccountId,
                ApprenticeshipId = apprenticeshipId,
                LearnerId = learnerId,
                StartDate = startDate,
                PlannedEndDate = plannedEndDate,
                ActualEndDate = actualEndDate,
                MonthlyInstallment = monthlyInstallment,
                CompletionAmount = completionAmount,
                NumberOfInstallments = numberOfInstallments
            };

            var results = _commitmentValidator.Validate(commitment);
            if (!results.IsValid)
            {
                _eventPublisher.Publish(results);
                return false;
            }

            var existingCommitment = _commitments.FirstOrDefault(c =>
                c.ApprenticeshipId == commitment.ApprenticeshipId && c.LearnerId == commitment.LearnerId);
            if (existingCommitment != null)
            {
                existingCommitment.ActualEndDate = actualEndDate;
                existingCommitment.MonthlyInstallment = monthlyInstallment;
                existingCommitment.PlannedEndDate = plannedEndDate;
                existingCommitment.StartDate = startDate;
                existingCommitment.CompletionAmount = completionAmount;
                existingCommitment.NumberOfInstallments = numberOfInstallments;
            }
            else
                _commitments.Add(commitment);

            return true;
        }

        public virtual Tuple<decimal, List<long>> GetTotalCostOfTraining(DateTime date)
        {
            var startOfMonth = date.GeStartOfMonth();
            var commitments = _commitments.Where(commitment =>
                    commitment.StartDate.GeStartOfMonth() <= startOfMonth &&
                    commitment.PlannedEndDate.GeStartOfMonth().AddMonths(-1) >= startOfMonth)
                .ToList();
            return new Tuple<decimal, List<long>>(commitments.Sum(c => c.MonthlyInstallment), commitments.Select(c => c.Id).ToList());
        }

        public virtual Tuple<decimal, List<long>> GetTotalCompletionPayments(DateTime date)
        {
            var startOfMonth = date.GeStartOfMonth();
            var commitments = _commitments.Where(commitment =>
                   commitment.PlannedEndDate.GeStartOfMonth() == startOfMonth)
                .ToList();
            return new Tuple<decimal, List<long>>(commitments.Sum(c => c.CompletionAmount), commitments.Select(c => c.Id).ToList());
        }
    }
}