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
        private readonly ICommitmentValidator _commitmentValidator;
        public ReadOnlyCollection<Commitment> Commitments => _commitments.AsReadOnly();


        public EmployerCommitments(
            long employerAccountId,
            List<Commitment> commitments,
            IEventPublisher eventPublisher,
            ICommitmentValidator commitmentValidator)
        {
            _employerAccountId = employerAccountId;
            _commitments = commitments ?? throw new ArgumentNullException(nameof(commitments));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
            _commitmentValidator = commitmentValidator;
        }

        public bool AddCommitment(long apprenticeshipId, long learnerId, string apprenticeName, string courseName, int? courseLevel,
            long providerId, string providerName, DateTime startDate, DateTime plannedEndDate,
            DateTime? actualEndDate, decimal monthlyInstallment, decimal completionAmount, short numberOfInstallments)
        {
            if (actualEndDate.HasValue && actualEndDate == DateTime.MinValue)
                actualEndDate = null;

            var commitment = new Commitment
            {
                EmployerAccountId = _employerAccountId,
                ApprenticeshipId = apprenticeshipId,
                ApprenticeName = apprenticeName,
                LearnerId = learnerId,
                CourseLevel = courseLevel,
                CourseName = courseName,
                ProviderId = providerId,
                ProviderName = providerName,
                StartDate = startDate,
                PlannedEndDate = plannedEndDate,
                ActualEndDate = actualEndDate,
                MonthlyInstallment = monthlyInstallment,
                CompletionAmount = completionAmount,
                NumberOfInstallments = numberOfInstallments,
            };

            if (!_commitmentValidator.IsValid(commitment))
            {
                //_eventPublisher.Publish(results);
                return false;
            }

            //TODO: Refactor and make simpler when we move to using EF rather than dapper
            var existingCommitment = _commitments.FirstOrDefault(c =>
                c.ApprenticeshipId == commitment.ApprenticeshipId);

            if (existingCommitment != null)
            {
                commitment.Id = existingCommitment.Id;
                _commitments.Remove(existingCommitment);
            }
            _commitments.Add(commitment);

            return true;
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