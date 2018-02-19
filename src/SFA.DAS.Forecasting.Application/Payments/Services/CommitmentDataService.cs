using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using NLog;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Domain.Payments.Services;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Payments;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.Forecasting.Application.Payments.Services
{
	public class CommitmentDataService : BaseRepository, ICommitmentDataService
	{

		public ILog Logger { get; }

		public CommitmentDataService(IApplicationConfiguration applicationConfiguration, ILog logger) : base(applicationConfiguration.DatabaseConnectionString, logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}


		public async Task StoreCommitment(Commitment commitment)
		{
			var txScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
			try
			{
				await WithConnection(async cnn =>
				{
					await StoreCommitment(cnn, commitment);
					return 0;
				});
				txScope.Complete();
			}
			catch (Exception ex)
			{
				Logger.Error(ex, $"Error storing commitment. Error: {ex}");
				throw;
			}
			finally
			{
				txScope.Dispose();
			}
		}

		private async Task StoreCommitment(IDbConnection connection, Commitment commitment)
		{
			var parameters = new DynamicParameters();
			parameters.Add("@employerAccountId", commitment.EmployerAccountId, DbType.Int64);
			parameters.Add("@apprenticeshipId", commitment.ApprenticeshipId, DbType.Int64);
			parameters.Add("@learnerId", commitment.LearnerId, DbType.Int64);
			parameters.Add("@startDate", commitment.StartDate, DbType.DateTime);
			parameters.Add("@plannedEndDate", commitment.PlannedEndDate, DbType.DateTime);
			parameters.Add("@actualEndDate", commitment.ActualEndDate, DbType.DateTime);
			parameters.Add("@completionAmount", commitment.CompletionAmount, DbType.Decimal);
			parameters.Add("@monthlyInstallment", commitment.MonthlyInstallment, DbType.Decimal);
			parameters.Add("@numberOfInstallments", commitment.NumberOfInstallments, DbType.Int16);
			parameters.Add("@providerId", commitment.ProviderId, DbType.Int64);
			parameters.Add("@providerName", commitment.ProviderName, DbType.String);
			parameters.Add("@apprenticeName", commitment.ApprenticeName, DbType.String);
			parameters.Add("@courseName", commitment.CourseName, DbType.String);
			parameters.Add("@courseLevel", commitment.CourseLevel, DbType.Int32);

			await connection.ExecuteAsync(
				@"MERGE Commitment AS target 
                                    USING(SELECT @employerAccountId, @apprenticeshipId, @learnerId, @startDate, @plannedEndDate, @actualEndDate, @completionAmount, @monthlyInstallment, @numberOfInstallments, @providerId, @providerName, @apprenticeName, @courseName, @courseLevel) 
									AS source(EmployerAccountId, ApprenticeshipId, LearnerId, StartDate, PlannedEndDate, ActualEndDate, CompletionAmount, MonthlyInstallment, NumberOfInstallments, ProviderId, ProviderName, ApprenticeName, CourseName, CourseLevel)
                                    ON(target.ApprenticeshipId = source.ApprenticeshipId)
                                    WHEN MATCHED THEN
                                        UPDATE SET StartDate = source.StartDate, PlannedEndDate = source.PlannedEndDate, 
													CompletionAmount = source.CompletionAmount, MonthlyInstallment = source.MonthlyInstallment,
													NumberOfInstallments = source.NumberOfInstallments, ProviderId = source.ProviderId,
													ProviderName = source.ProviderName, ApprenticeName = source.ApprenticeName,
													CourseName = source.CourseName, CourseLevel = source.CourseLevel,
													LearnerId = source.LearnerId, EmployerAccountId = source.EmployerAccountId
                                    WHEN NOT MATCHED THEN
                                        INSERT(EmployerAccountId, ApprenticeshipId, LearnerId, StartDate, PlannedEndDate, ActualEndDate, CompletionAmount, MonthlyInstallment, NumberOfInstallments, ProviderId, ProviderName, ApprenticeName, CourseName, CourseLevel)
                                        VALUES(source.EmployerAccountId, source.ApprenticeshipId, source.LearnerId, source.StartDate, source.PlannedEndDate, source.ActualEndDate, source.CompletionAmount, source.MonthlyInstallment, source.NumberOfInstallments, source.ProviderId, source.ProviderName, source.ApprenticeName, source.CourseName, source.CourseLevel);",
				parameters,
				commandType: CommandType.Text);
		}
	}
}
