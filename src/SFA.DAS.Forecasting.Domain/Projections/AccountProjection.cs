using System;
using System.Collections.Generic;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Projections.Model;
using SFA.DAS.Forecasting.ReadModel.Projections;

namespace SFA.DAS.Forecasting.Domain.Projections
{
    public class AccountProjection
    {
        private readonly AccountActivity _activity;
        private readonly EmployerCommitments _employerCommitments;

        public AccountProjection(AccountActivity activity, EmployerCommitments employerCommitments)
        {
            _activity = activity ?? throw new ArgumentNullException(nameof(activity));
            _employerCommitments = employerCommitments ?? throw new ArgumentNullException(nameof(employerCommitments));
        }

        public List<ReadModel.Projections.AccountProjection> GetLevyTriggeredProjections(DateTime periodStart, int numberOfMonths)
        {
            return GetProjections(periodStart, numberOfMonths, ProjectionGenerationType.LevyDeclaration);
        }

        public List<ReadModel.Projections.AccountProjection> GetPayrollPeriodEndTriggeredProjections(DateTime periodStart, int numberOfMonths)
        {
            return GetProjections(periodStart, numberOfMonths, ProjectionGenerationType.PayrollPeriodEnd);
        }

        private List<ReadModel.Projections.AccountProjection> GetProjections(DateTime periodStart, int numberOfMonths, ProjectionGenerationType projectionGenerationType)
        {
            var projections = new List<ReadModel.Projections.AccountProjection>();
            var lastBalance = _activity.Balance;
            for (var i = 1; i <= numberOfMonths; i++)
            {
                var projection = CreateProjection(periodStart.AddMonths(i),
                    projectionGenerationType == ProjectionGenerationType.LevyDeclaration && i == 1
                        ? 0
                        : _activity.LevyDeclared, lastBalance, ProjectionGenerationType.LevyDeclaration);
                projections.Add(projection);
                lastBalance = projection.FutureFunds;
            }
            return projections;
        }

        private ReadModel.Projections.AccountProjection CreateProjection(DateTime period, decimal fundsIn, decimal lastBalance, ProjectionGenerationType projectionGenerationType)
        {
            var totalCostOfTraning = _employerCommitments.GetTotalCostOfTraining(period);
            var completionPayments = _employerCommitments.GetTotalCompletionPayments(period);
            var projection = new ReadModel.Projections.AccountProjection
            {
                FundsIn = _activity.LevyDeclared,
                EmployerAccountId = _activity.EmployerAccountId,
                Month = (short)period.Month,
                Year = (short)period.Year,
                TotalCostOfTraining = totalCostOfTraning,
                CompletionPayments = completionPayments,
                FutureFunds = lastBalance + fundsIn - totalCostOfTraning - completionPayments,
                ProjectionCreationDate = DateTime.UtcNow,
                ProjectionGenerationType = projectionGenerationType,
            };
            return projection;

        }
    }
}