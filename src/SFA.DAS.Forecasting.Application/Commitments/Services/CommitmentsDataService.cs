using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Domain.Commitments.Services;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.Forecasting.Application.Commitments.Services
{
    public class CommitmentsDataService : BaseRepository, ICommitmentsDataService
    {
        public CommitmentsDataService(IApplicationConfiguration configuration, ILog logger) : base(configuration.DatabaseConnectionString, logger)
        {
        }

        public async Task<List<Commitment>> GetCurrentCommitments(long employerAccountId)
        {
            return await WithConnection(async cnn =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@employerAccountId", employerAccountId, DbType.Int64);

                var sql = @"Select 
                                [Id],
	                            [EmployerAccountId],
	                            [LearnerId],
	                            [ProviderId],
	                            [ProviderName],	
	                            [ApprenticeshipId],
                                [ApprenticeName],
                                [CourseName],
                                [CourseLevel],
	                            [StartDate],
                                [PlannedEndDate],
                                [ActualEndDate],
                                [CompletionAmount],
                                [MonthlyInstallment],
                                [NumberOfInstallments]
                                From Commitment
                                Where EmployerAccountId = @employerAccountId and ActualEndDate Is Null";

                var commitments = await cnn.QueryAsync<Commitment>(
                    sql,
                    parameters,
                    commandType: CommandType.Text);
                return commitments.ToList();
            });
        }

        public async Task<decimal> GetOverdueCompletionPayments(long employerAccountId)
        {
            return await WithConnection(async cnn =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@employerAccountId", employerAccountId, DbType.Int64);

                var sql = @"SELECT sum(CompletionAmount) FROM [SFA.DAS.Forecasting.Database].[dbo].[Commitment]
                        WHERE ActualEndDate is null
                        and PlannedEndDate< '2019-04-01'
                        and EmployerAccountId = @employerAccountId";

                var val = await cnn.QueryAsync<decimal>(
                    sql,
                    parameters,
                    commandType: CommandType.Text);
                return val.FirstOrDefault();
            });
        }

        public async Task Store(IEnumerable<Commitment> commitments)
        {
            using (var txScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await WithConnection(async cnn =>
                {
                    foreach (var commitment in commitments)
                        await StoreCommitment(cnn, commitment);
                    return 0;
                });
                txScope.Complete();
            }
        }

        private async Task StoreCommitment(IDbConnection connection, Commitment commitment)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@employerAccountId", commitment.EmployerAccountId, DbType.Int64);
            parameters.Add("@learnerId", commitment.LearnerId, DbType.Int64);
            parameters.Add("@providerId", commitment.ProviderId, DbType.Int64);
            parameters.Add("@providerName", commitment.ProviderName, DbType.String);
            parameters.Add("@apprenticeshipId", commitment.ApprenticeshipId, DbType.Int64);
            parameters.Add("@apprenticeName", commitment.ApprenticeName, DbType.String);
            parameters.Add("@courseName", commitment.CourseName, DbType.String);
            parameters.Add("@courseLevel", commitment.CourseLevel, DbType.Int32);
            parameters.Add("@startDate", commitment.StartDate, DbType.DateTime);
            parameters.Add("@plannedEndDate", commitment.PlannedEndDate, DbType.DateTime);
            parameters.Add("@actualEndDate", commitment.ActualEndDate, DbType.DateTime);
            parameters.Add("@completionAmount", commitment.CompletionAmount, DbType.Decimal, ParameterDirection.Input, null, 18, 2);
            parameters.Add("@monthlyInstallment", commitment.MonthlyInstallment, DbType.Decimal, ParameterDirection.Input, null, 18, 2);
            parameters.Add("@numberOfInstallments", commitment.NumberOfInstallments, DbType.Int32);

            await connection.ExecuteAsync(
                @"MERGE Commitment AS target 
                                    USING(SELECT @employerAccountId, @learnerId, @providerId, @providerName, @apprenticeshipId, @apprenticeName, @courseName, @courseLevel, @startDate, @plannedEndDate, @actualEndDate, @completionAmount, @monthlyInstallment, @numberOfInstallments) AS source (EmployerAccountId, LearnerId, ProviderId, ProviderName, ApprenticeshipId, ApprenticeName, CourseName, CourseLevel, StartDate, PlannedEndDate, ActualEndDate, CompletionAmount, MonthlyInstallment, NumberOfInstallments)
                                    ON (target.EmployerAccountId = source.EmployerAccountId and target.ApprenticeshipId = source.ApprenticeshipId)
                                    WHEN MATCHED THEN
                                        UPDATE SET 
                                            ProviderId = source.ProviderId, 
                                            ProviderName = source.ProviderName, 
                                            ApprenticeshipId = source.ApprenticeshipId, 
                                            ApprenticeName = source.ApprenticeName , 
                                            CourseName = source.CourseName , 
                                            CourseLevel = source.CourseLevel , 
                                            StartDate = source.StartDate , 
                                            PlannedEndDate = source.PlannedEndDate , 
                                            ActualEndDate = source.ActualEndDate , 
                                            CompletionAmount = source.CompletionAmount , 
                                            MonthlyInstallment = source.MonthlyInstallment , 
                                            NumberOfInstallments = source.NumberOfInstallments     
                                    WHEN NOT MATCHED THEN
                                        INSERT(EmployerAccountId, LearnerId, ProviderId, ProviderName, ApprenticeshipId, ApprenticeName, CourseName, CourseLevel, StartDate, PlannedEndDate, ActualEndDate, CompletionAmount, MonthlyInstallment, NumberOfInstallments)
                                        VALUES(source.EmployerAccountId, source.LearnerId, source.ProviderId, source.ProviderName, source.ApprenticeshipId, source.ApprenticeName, source.CourseName, source.CourseLevel, source.StartDate, source.PlannedEndDate, source.ActualEndDate, source.CompletionAmount, source.MonthlyInstallment, source.NumberOfInstallments);",
                parameters,
                commandType: CommandType.Text);
        }
    }
}