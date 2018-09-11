using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Data;
using SFA.DAS.Forecasting.Domain.Projections.Services;
using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Application.Projections.Services
{
    public class AccountProjectionDataSession : IAccountProjectionDataSession
    {
        private readonly IForecastingDataContext _dataContext;
        private readonly ITelemetry _telemetry;

        public AccountProjectionDataSession(IForecastingDataContext dataContext, ITelemetry telemetry)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public async Task<List<AccountProjectionModel>> Get(long employerAccountId)
        {
            return await _dataContext.AccountProjections
                .Where(projection => projection.EmployerAccountId == employerAccountId)
                .ToListAsync();
        }

        public async Task Store(IEnumerable<AccountProjectionModel> accountProjections)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var startTime = DateTime.UtcNow;
            var insertString = new StringBuilder();
            var accountCommitmentsInsert =
                "DECLARE @TempAccountProjection as TABLE" +
                "(" +
                "	[EmployerAccountId] BIGINT NOT NULL," +
                "   [ProjectionCreationDate] DATETIME NOT NULL," +
                "   [ProjectionGenerationType] TINYINT NOT NULL," +
                "   [Month] SMALLINT NOT NULL," +
                "   [Year] INT NOT NULL," +
                "   [FundsIn] DECIMAL(18,5) NOT NULL," +
                "   [TotalCostOfTraining] DECIMAL(18,5) NOT NULL," +
                "	[TransferOutTotalCostOfTraining] DECIMAL(18,5) NOT NULL default(0)," +
                "	[TransferInTotalCostOfTraining] DECIMAL(18,5) NOT NULL default(0)," +
                "	[TransferInCompletionPayments] DECIMAL(18,5) NOT NULL default(0)," +
                "   [CompletionPayments] DECIMAL(18,5) NOT NULL," +
                "	[TransferOutCompletionPayments] DECIMAL(18,5) NOT NULL default(0)," +
                "   [FutureFunds] DECIMAL(18,5) NOT NULL," +
                "	[CoInvestmentEmployer] DECIMAL(18,5) NOT NULL default(0)," +
                "	[CoInvestmentGovernment] DECIMAL(18,5) NOT NULL default(0)" +
                ") ";


            _dataContext.Configuration.AutoDetectChangesEnabled = false;
            insertString.Append(accountCommitmentsInsert);
            insertString.AppendLine("insert into @TempAccountProjection VALUES ");
            foreach (var accountProjectionModel in accountProjections)
            {
                insertString.AppendLine($"({accountProjectionModel.EmployerAccountId}," +
                                        $"'{accountProjectionModel.ProjectionCreationDate:yyyy-MM-dd HH:mm:ss.fff}'," +
                                        $"{(byte)accountProjectionModel.ProjectionGenerationType}," +
                                        $"{accountProjectionModel.Month}," +
                                        $"{accountProjectionModel.Year}," +
                                        $"{accountProjectionModel.LevyFundsIn}," + //check
                                        $"{accountProjectionModel.LevyFundedCostOfTraining}," + //check
                                        $"{accountProjectionModel.TransferOutCostOfTraining}," +
                                        $"{accountProjectionModel.TransferInCostOfTraining}," +
                                        $"{accountProjectionModel.TransferInCompletionPayments}," +
                                        $"{accountProjectionModel.LevyFundedCompletionPayments}," +
                                        $"{accountProjectionModel.TransferOutCompletionPayments}," +
                                        $"{accountProjectionModel.FutureFunds}," +
                                        $"{accountProjectionModel.CoInvestmentEmployer}," +
                                        $"{accountProjectionModel.CoInvestmentGovernment}" +
                                        "),");
            }

            var insertStatement = insertString.ToString().Trim().TrimEnd(',');

            insertString = new StringBuilder();
            insertString.AppendLine(insertStatement);
            insertString.AppendLine("");
            insertString.AppendLine(" MERGE accountprojection t " +
                                    " USING (" +
                                    "   select " +
                                    " 		*" +
                                    "     from @TempAccountProjection s" +
                                    " ) s" +
                                    " ON t.employeraccountid = s.employeraccountid and t.[month] = s.[month] and t.[year] = s.[year] " +
                                    " WHEN NOT MATCHED THEN " +
                                    "     INSERT (EmployerAccountId, ProjectionCreationDate, ProjectionGenerationType, [Month], [Year], FundsIn, TotalCostOfTraining, " +
                                    " 			TransferOutTotalCostOfTraining, TransferInTotalCostOfTraining, TransferInCompletionPayments, CompletionPayments, " +
                                    " 			TransferOutCompletionPayments,FutureFunds,CoinvestmentEmployer, CoInvestmentGovernment) " +
                                    "     VALUES (s.EmployerAccountId,s.ProjectionCreationDate,s.ProjectionGenerationType,s.[Month], " +
                                    " 			s.[Year],s.fundsIn,s.TotalCostOfTraining,s.TransferOutTotalCostOfTraining,s.TransferInTotalCostOfTraining, s.TransferInCompletionPayments, " +
                                    " 			s.CompletionPayments, s.TransferOutCompletionPayments, s.FutureFunds, s.CoInvestmentEmployer, s.CoInvestmentGovernment) " +
                                    " WHEN MATCHED THEN " +
                                    " 	UPDATE SET ProjectionCreationDate = s.ProjectionCreationDate, FundsIn = s.FundsIn, TotalCostOfTraining = s.TotalCostOfTraining, " +
                                    " 				TransferOutTotalCostOfTraining = s.TransferOutTotalCostOfTraining,TransferInTotalCostOfTraining = s.TransferInTotalCostOfTraining, " +
                                    " 				TransferInCompletionPayments = s.TransferInCompletionPayments, CompletionPayments = s.CompletionPayments, " +
                                    " 				TransferOutCompletionPayments = s.TransferOutCompletionPayments, FutureFunds = s.FutureFunds, " +
                                    " 				CoinvestmentEmployer = s.CoinvestmentEmployer, CoInvestmentGovernment = s.CoInvestmentGovernment;");

            await _dataContext.Database.ExecuteSqlCommandAsync(insertString.ToString());
            stopwatch.Stop();
            _telemetry.TrackDependency(DependencyType.SqlDatabaseMerge, "Store Account Projections", startTime, stopwatch.Elapsed, true);
        }

        public async Task DeleteAll(long employerAccountId)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var startTime = DateTime.UtcNow;

            await _dataContext.Database.ExecuteSqlCommandAsync("DELETE FROM dbo.AccountProjection where EmployerAccountId=@p0", employerAccountId);
            stopwatch.Stop();
            _telemetry.TrackDependency(DependencyType.SqlDatabaseDelete, "Delete Account Projections", startTime, stopwatch.Elapsed, true);
        }

        public async Task SaveChanges()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var startTime = DateTime.UtcNow;
            await _dataContext.SaveChangesAsync();
            _telemetry.TrackDependency("EF Context Save Changes", "Account Projections", startTime, stopwatch.Elapsed, true);

        }
    }
}
