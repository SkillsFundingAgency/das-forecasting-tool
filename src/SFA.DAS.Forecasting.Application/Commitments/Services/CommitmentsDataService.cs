using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Data;
using SFA.DAS.Forecasting.Domain.Commitments.Services;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Application.Commitments.Services
{
    public class CommitmentsDataService : ICommitmentsDataService
    {
        private readonly IForecastingDataContext _dataContext;
        private readonly ILogger<CommitmentsDataService> _logger;

        public CommitmentsDataService(IForecastingDataContext dataContext, ILogger<CommitmentsDataService> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
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

        public async Task DeletePledgeApplicationCommitmentsForSendingEmployer(long employerAccountId)
        {
            var pledgeApplicationCommitments = _dataContext.Commitments.Where(x =>
                x.SendingEmployerAccountId == employerAccountId &&
                (x.FundingSource == FundingSource.ApprovedPledgeApplication ||
                 x.FundingSource == FundingSource.AcceptedPledgeApplication));

            _dataContext.Commitments.RemoveRange(pledgeApplicationCommitments);
            await Task.FromResult(_dataContext.SaveChanges());
        }

        public async Task<EmployerCommitmentsModel> GetCurrentCommitments(long employerAccountId, DateTime? forecastLimitDate = null)
        {
            var model = new EmployerCommitmentsModel
            {
                LevyFundedCommitments = await GetCommitments(GetLevyFundedCommiments(employerAccountId, forecastLimitDate), "Levy Funded Commitments"),
                ReceivingEmployerTransferCommitments = await GetCommitments(GetReceivingEmployerTransferCommitments(employerAccountId, forecastLimitDate),"Receiving Employer Transfer Commitments"),
                SendingEmployerTransferCommitments = await GetCommitments(GetSendingEmployerTransferCommitments(employerAccountId, forecastLimitDate),"Sending Employer Transfer Commitments"),
                CoInvestmentCommitments = await GetCommitments(GetCoInvestmentCommitments(employerAccountId),"Co-Investment Commitments")
            };
            return model;
        }

        private async Task<List<CommitmentModel>> GetCommitments(Expression<Func<CommitmentModel, bool>> filter, string queryName)
        {
            var listSize = await _dataContext.Commitments.AsNoTracking().CountAsync(filter);
            
            var model = new List<CommitmentModel>(listSize);

            if (listSize == 0)
            {
                return model;
            }

            var moreData = true;
            var skip = 0;
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

            return model;
        }

        private Expression<Func<CommitmentModel, bool>> GetLevyFundedCommiments(long employerAccountId, DateTime? forecastLimitDate)
        {
            return commitment => commitment.EmployerAccountId == employerAccountId
                                 && commitment.FundingSource == FundingSource.Levy
                                 && (!forecastLimitDate.HasValue || commitment.StartDate <= forecastLimitDate)
                                 && (commitment.ActualEndDate == null || commitment.ActualEndDate > DateTime.UtcNow);
        }

        private Expression<Func<CommitmentModel, bool>> GetReceivingEmployerTransferCommitments(long employerAccountId, DateTime? forecastLimitDate)
        {
            return commitment => commitment.EmployerAccountId == employerAccountId
                                 && commitment.FundingSource == FundingSource.Transfer
                                 && (!forecastLimitDate.HasValue || commitment.StartDate <= forecastLimitDate)
                                 && (commitment.ActualEndDate == null || commitment.ActualEndDate > DateTime.UtcNow);
        }

        private Expression<Func<CommitmentModel, bool>> GetSendingEmployerTransferCommitments(long employerAccountId, DateTime? forecastLimitDate)
        {
            return commitment => commitment.SendingEmployerAccountId == employerAccountId
                                 && (commitment.FundingSource == FundingSource.Transfer ||
                                     commitment.FundingSource == FundingSource.ApprovedPledgeApplication ||
                                     commitment.FundingSource == FundingSource.AcceptedPledgeApplication)
                                 && (!forecastLimitDate.HasValue || commitment.StartDate <= forecastLimitDate)
                                 && (commitment.ActualEndDate == null || commitment.ActualEndDate > DateTime.UtcNow);
        }

        private Expression<Func<CommitmentModel, bool>> GetCoInvestmentCommitments(long employerAccountId)
        {
            return commitment => commitment.EmployerAccountId == employerAccountId
                                 && (commitment.FundingSource == FundingSource.CoInvestedEmployer ||
                                     commitment.FundingSource == FundingSource.CoInvestedSfa)
                                 && (commitment.ActualEndDate == null || commitment.ActualEndDate > DateTime.UtcNow);
        }

        public async Task<CommitmentModel> Get(long employerAccountId, long apprenticeshipId)
        {
            var commitmentModel = await _dataContext.Commitments.FirstOrDefaultAsync(commitment =>
                commitment.EmployerAccountId == employerAccountId &&
                commitment.ApprenticeshipId == apprenticeshipId);
            
            return commitmentModel;
        }

        public async Task Store(CommitmentModel commitment)
        {
            try
            {
                _logger.LogInformation($"Store Commitment Id {commitment.Id}");
                var startTime = DateTime.UtcNow;
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                if (commitment.Id <= 0)
                    await _dataContext.Commitments.AddAsync(commitment);

                _dataContext.SaveChanges();

            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error adding commitments");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,$"Store Commitment Failed for CommitmentId {commitment.Id}");
            }
        }

    }
}