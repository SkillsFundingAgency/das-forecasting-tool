using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Forecasting.Data;
using SFA.DAS.Forecasting.Domain.Projections.Services;
using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Application.Projections.Services
{
    public class AccountProjectionDataSession : IAccountProjectionDataSession
    {
        private readonly IForecastingDataContext _dataContext;
        

        public AccountProjectionDataSession(IForecastingDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<AccountProjectionModel>> Get(long employerAccountId)
        {
            return await _dataContext.AccountProjections
                .Where(projection => projection.EmployerAccountId == employerAccountId)
                .ToListAsync();
        }

        public async Task Store(IEnumerable<AccountProjectionModel> accountProjections)
        {
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
                "	[CoInvestmentGovernment] DECIMAL(18,5) NOT NULL default(0)," +
                "	[ExpiredFunds] DECIMAL(18,5) NOT NULL default(0)," +
                "	[FutureFundsNoExpiry] DECIMAL(18,5) NOT NULL default(0)," +
                "	[ApprovedPledgeApplicationCost] DECIMAL(18,5) NOT NULL default(0)," +
                "	[AcceptedPledgeApplicationCost] DECIMAL(18,5) NOT NULL default(0)," +
                "	[PledgeOriginatedCommitmentCost] DECIMAL(18,5) NOT NULL default(0)" +
                ") ";


            //_dataContext.Configuration.AutoDetectChangesEnabled = false;
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
                                        $"{accountProjectionModel.CoInvestmentGovernment}," +
                                        $"{accountProjectionModel.ExpiredFunds}," +
                                        $"{accountProjectionModel.FutureFundsNoExpiry}," +
                                        $"{accountProjectionModel.ApprovedPledgeApplicationCost}," +
                                        $"{accountProjectionModel.AcceptedPledgeApplicationCost}," +
                                        $"{accountProjectionModel.PledgeOriginatedCommitmentCost}" +
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
                                    " 			TransferOutCompletionPayments,FutureFunds,CoinvestmentEmployer, CoInvestmentGovernment, ExpiredFunds, FutureFundsNoExpiry, ApprovedPledgeApplicationCost, AcceptedPledgeApplicationCost, PledgeOriginatedCommitmentCost) " +
                                    "     VALUES (s.EmployerAccountId,s.ProjectionCreationDate,s.ProjectionGenerationType,s.[Month], " +
                                    " 			s.[Year],s.fundsIn,s.TotalCostOfTraining,s.TransferOutTotalCostOfTraining,s.TransferInTotalCostOfTraining, s.TransferInCompletionPayments, " +
                                    " 			s.CompletionPayments, s.TransferOutCompletionPayments, s.FutureFunds, s.CoInvestmentEmployer, s.CoInvestmentGovernment, s.ExpiredFunds, s.FutureFundsNoExpiry, s.ApprovedPledgeApplicationCost, s.AcceptedPledgeApplicationCost, s.PledgeOriginatedCommitmentCost) " +
                                    " WHEN MATCHED THEN " +
                                    " 	UPDATE SET ProjectionCreationDate = s.ProjectionCreationDate, FundsIn = s.FundsIn, TotalCostOfTraining = s.TotalCostOfTraining, " +
                                    " 				TransferOutTotalCostOfTraining = s.TransferOutTotalCostOfTraining,TransferInTotalCostOfTraining = s.TransferInTotalCostOfTraining, " +
                                    " 				TransferInCompletionPayments = s.TransferInCompletionPayments, CompletionPayments = s.CompletionPayments, " +
                                    " 				TransferOutCompletionPayments = s.TransferOutCompletionPayments, FutureFunds = s.FutureFunds, " +
                                    " 				CoinvestmentEmployer = s.CoinvestmentEmployer, CoInvestmentGovernment = s.CoInvestmentGovernment, ExpiredFunds = s.ExpiredFunds, FutureFundsNoExpiry = s.FutureFundsNoExpiry, ApprovedPledgeApplicationCost = s.ApprovedPledgeApplicationCost, AcceptedPledgeApplicationCost = s.AcceptedPledgeApplicationCost, PledgeOriginatedCommitmentCost = s.PledgeOriginatedCommitmentCost;");

            await _dataContext.ExecuteRawSql(insertString.ToString());
            
        }

        public async Task DeleteAll(long employerAccountId)
        {
            await _dataContext.ExecuteSqlInterpolatedAsync($"DELETE FROM dbo.AccountProjection where EmployerAccountId={employerAccountId}" );
        }

        public Task SaveChanges()
        {
            return Task.FromResult(_dataContext.SaveChanges());
        }
    }
}
