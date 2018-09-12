using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Data;
using SFA.DAS.Forecasting.Domain.Commitments.Services;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Payments;

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
                .Commitments.Where(commitment => commitment.EmployerAccountId == employerAccountId)
                .OrderByDescending(commitment => commitment.ReceivedTime)
                .Select(commitment => commitment.ReceivedTime)
                .FirstOrDefaultAsync();
        }

        public async Task<EmployerCommitmentsModel> GetCurrentCommitments(long employerAccountId, DateTime? forecastLimitDate = null)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var model = new EmployerCommitmentsModel
            {
                LevyFundedCommitments = await GetCommitments(GetLevyFundedCommiments(employerAccountId, forecastLimitDate), "Levy Funded Commitments"),
                ReceivingEmployerTransferCommitments = await GetCommitments(GetReceivingEmployerTransferCommitments(employerAccountId, forecastLimitDate), "Receiving Employer Transfer Commitments"),
                SendingEmployerTransferCommitments = await GetCommitments(GetSendingEmployerTransferCommitments(employerAccountId, forecastLimitDate), "Sending Employer Transfer Commitments"),
                CoInvestmentCommitments = await GetCommitments(GetCoInvestmentCommitments(employerAccountId), "Co-Investment Commitments")
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
                                 && (commitment.FundingSource == FundingSource.CoInvestedEmployer || commitment.FundingSource == FundingSource.CoInvestedSfa)
                                 && commitment.ActualEndDate == null;
        }

        public async Task Upsert(CommitmentModel commitment)
        {
            var sql = UpsertSqlString();
            var parameters = new object[]
            {
                new SqlParameter("@employerAccountId", commitment.EmployerAccountId),
                new SqlParameter("@sendingEmployerAccountId", commitment.SendingEmployerAccountId),
                new SqlParameter("@learnerId", commitment.LearnerId),
                new SqlParameter("@providerId", commitment.ProviderId),

                new SqlParameter("@providerName", string.IsNullOrEmpty(commitment.ProviderName) ? "No Name Provided" : commitment.ProviderName),
                new SqlParameter("@apprenticeshipId", commitment.ApprenticeshipId),
                new SqlParameter("@apprenticeName", commitment.ApprenticeName),

                new SqlParameter("@courseName", commitment.CourseName),
                new SqlParameter("@courseLevel", commitment.CourseLevel),
                new SqlParameter("@startDate", commitment.StartDate)
                {
                    SqlDbType = SqlDbType.DateTime
                },
                new SqlParameter("@plannedEndDate", commitment.PlannedEndDate)
                {
                    SqlDbType = SqlDbType.DateTime
                },
                new SqlParameter("@actualEndDate", commitment.ActualEndDate ?? (object) DBNull.Value)
                    {
                        IsNullable = true,
                        SqlDbType = System.Data.SqlDbType.DateTime,
                        Direction = System.Data.ParameterDirection.Input
                    },
                new SqlParameter("@completionAmount", commitment.CompletionAmount),
                new SqlParameter("@monthlyInstallment", commitment.MonthlyInstallment),
                new SqlParameter("@numberOfInstallments", commitment.NumberOfInstallments),
                new SqlParameter("@fundingSource", commitment.FundingSource),
                new SqlParameter("@updatedDateTime", commitment.UpdatedDateTime)
                {
                    SqlDbType = SqlDbType.DateTime
                },
                new SqlParameter("@hasHadPayment", commitment.HasHadPayment)
            };

            await _dataContext.Database.ExecuteSqlCommandAsync(sql, parameters);

        }

        private string UpsertSqlString()
        {
            return @"
                    MERGE INTO Commitment
                    USING 
                    (
                        SELECT @employerAccountId        as EmployerAccountId,
		                       @sendingEmployerAccountId as SendingEmployerAccountId,
		                       @learnerId                as LearnerId,
		                       @providerId               as ProviderId,
		                       @providerName             as ProviderName,
		                       @apprenticeshipId         as ApprenticeshipId,
		                       @apprenticeName           as ApprenticeName,
		                       @courseName               as CourseName,
		                       @courseLevel              as CourseLevel,
		                       @startDate                as StartDate,
		                       @plannedEndDate           as PlannedEndDate,
		                       @actualEndDate            as ActualEndDate,
		                       @completionAmount         as CompletionAmount,
		                       @monthlyInstallment       as MonthlyInstallment,
		                       @numberOfInstallments     as NumberOfInstallments,
		                       @fundingSource            as FundingSource,
                               @updatedDateTime          as UpdatedDateTime,
                               @hasHadPayment            as HasHadPayment
                    ) AS entity
                    ON  Commitment.EmployerAccountId = entity.EmployerAccountId 
                        AND Commitment.learnerId = entity.LearnerId
                    WHEN MATCHED 
	                    THEN
                        UPDATE 
                        SET 
		                    ApprenticeshipId = entity.ApprenticeshipId,
		                    ApprenticeName = entity.ApprenticeName,
		                    ActualEndDate = entity.ActualEndDate,
                            HasHadPayment = entity.HasHadPayment,
                            UpdatedDateTime = entity.UpdatedDateTime
                    WHEN NOT MATCHED 
                        AND entity.ActualEndDate is null 
                        THEN 
                        INSERT (EmployerAccountId,SendingEmployerAccountId,LearnerId,ProviderId,ProviderName,ApprenticeshipId,ApprenticeName,CourseName,CourseLevel,StartDate,PlannedEndDate,ActualEndDate,CompletionAmount,MonthlyInstallment,NumberOfInstallments,FundingSource, HasHadPayment,UpdatedDateTime)
	                    VALUES (
                                entity.EmployerAccountId,
                                entity.SendingEmployerAccountId,
                                entity.LearnerId, 
                                entity.ProviderId, 
                                entity.ProviderName,
                                entity.ApprenticeshipId, 
                                entity.ApprenticeName, 
                                entity.CourseName, 
                                entity.CourseLevel, 
                                entity.StartDate, 
                                entity.PlannedEndDate, 
                                entity.ActualEndDate, 
                                entity.CompletionAmount, 
                                entity.MonthlyInstallment, 
                                entity.NumberOfInstallments, 
                                entity.FundingSource,
                                entity.UpdatedDateTime,
                                entity.HasHadPayment);
                    ";

        }
    }
}