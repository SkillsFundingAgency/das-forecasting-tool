using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Data;
using SFA.DAS.Forecasting.Domain.Commitments.Services;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Payments;
using IsolationLevel = System.Data.IsolationLevel;

namespace SFA.DAS.Forecasting.Application.Commitments.Services
{
    public class CommitmentsDataService : ICommitmentsDataService
    {
        private readonly IForecastingDataContext _dataContext;
        private readonly ITelemetry _telemetry;

        public CommitmentsDataService(IForecastingDataContext dataContext, ITelemetry telemetry)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }
        public async Task<DateTime?> GetLastReceivedTime(long employerAccountId)
        {
            return await _dataContext

                .Commitments.Where(commitment => commitment.EmployerAccountId == employerAccountId 
                                                 || commitment.SendingEmployerAccountId == employerAccountId)
                .OrderByDescending(commitment => commitment.UpdatedDateTime)
                .Select(commitment => commitment.UpdatedDateTime)
                .FirstOrDefaultAsync();
        }

        public async Task<EmployerCommitmentsModel> GetCurrentCommitments(long employerAccountId, DateTime? forecastLimitDate = null)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var model = new EmployerCommitmentsModel
            {
                LevyFundedCommitments = await GetCommitments(GetLevyFundedCommiments(employerAccountId, forecastLimitDate), "Levy Funded Commitments"),
                ReceivingEmployerTransferCommitments = await GetCommitments(GetReceivingEmployerTransferCommitments(employerAccountId, forecastLimitDate),"Receiving Employer Transfer Commitments"),
                SendingEmployerTransferCommitments = await GetCommitments(GetSendingEmployerTransferCommitments(employerAccountId, forecastLimitDate),"Sending Employer Transfer Commitments"),
                CoInvestmentCommitments = await GetCommitments(GetCoInvestmentCommitments(employerAccountId),"Co-Investment Commitments")
            };
            stopwatch.Stop();
            _telemetry.TrackDuration("Get Current Commitments", stopwatch.Elapsed);
            return model;
        }

        private async Task<List<CommitmentModel>> GetCommitments(Expression<Func<CommitmentModel, bool>> filter, string queryName)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var listSize = await _dataContext.Commitments.AsNoTracking().CountAsync(filter);
            stopwatch.Stop();
            _telemetry.TrackDuration("Get Commitment Count: " + queryName, stopwatch.Elapsed);
            var model = new List<CommitmentModel>(listSize);

            if (listSize == 0)
            {
                return model;
            }

            var moreData = true;
            var skip = 0;
            stopwatch.Restart();
            while (moreData)
            {
                var commitments = await _dataContext.Commitments.AsNoTracking().Where(filter).OrderBy(c => c.Id).Skip(skip).Take(100).ToListAsync();

                skip = skip + 100;
                if (!commitments.Any())
                {
                    moreData = false;
                }
                else
                {
                    model.AddRange(commitments);
                }
            }

            stopwatch.Stop();
            _telemetry.TrackDuration($"Get {queryName}", stopwatch.Elapsed);
            return model;
        }

        private Expression<Func<CommitmentModel, bool>> GetLevyFundedCommiments(long employerAccountId, DateTime? forecastLimitDate)
        {
            return commitment => commitment.EmployerAccountId == employerAccountId
                                 && commitment.FundingSource == FundingSource.Levy
                                 && (!forecastLimitDate.HasValue || commitment.StartDate <= forecastLimitDate)
                                 && commitment.ActualEndDate == null;
        }

        private Expression<Func<CommitmentModel, bool>> GetReceivingEmployerTransferCommitments(long employerAccountId, DateTime? forecastLimitDate)
        {
            return commitment => commitment.EmployerAccountId == employerAccountId
                                 && commitment.FundingSource == FundingSource.Transfer
                                 && (!forecastLimitDate.HasValue || commitment.StartDate <= forecastLimitDate)
                                 && commitment.ActualEndDate == null;
        }

        private Expression<Func<CommitmentModel, bool>> GetSendingEmployerTransferCommitments(long employerAccountId, DateTime? forecastLimitDate)
        {
            return commitment => commitment.SendingEmployerAccountId == employerAccountId
                                 && commitment.FundingSource == FundingSource.Transfer
                                 && (!forecastLimitDate.HasValue || commitment.StartDate <= forecastLimitDate)
                                 && commitment.ActualEndDate == null;
        }

        private Expression<Func<CommitmentModel, bool>> GetCoInvestmentCommitments(long employerAccountId)
        {
            return commitment => commitment.EmployerAccountId == employerAccountId
                                 && (commitment.FundingSource == FundingSource.CoInvestedEmployer ||
                                     commitment.FundingSource == FundingSource.CoInvestedSfa)
                                 && commitment.ActualEndDate == null;
        }

        public async Task<CommitmentModel> Get(long employerAccountId, long apprenticeshipId)
        {
            var startTime = DateTime.UtcNow;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var commitmentModel = await _dataContext.Commitments.FirstOrDefaultAsync(commitment =>
                commitment.EmployerAccountId == employerAccountId &&
                commitment.ApprenticeshipId == apprenticeshipId);
            stopwatch.Stop();
            _telemetry.TrackDependency(DependencyType.SqlDatabaseQuery, "Get Commitment", startTime, stopwatch.Elapsed,
                commitmentModel != null);
            return commitmentModel;
        }

        public async Task Store(CommitmentModel commitment)
        {
            var startTime = DateTime.UtcNow;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            if (commitment.Id <= 0)
                _dataContext.Commitments.Add(commitment);
    
            await _dataContext.SaveChangesAsync();
        
            stopwatch.Stop();

            _telemetry.TrackDependency(DependencyType.SqlDatabaseQuery, "Store Commitment", startTime, stopwatch.Elapsed, true);

        }

    }
}