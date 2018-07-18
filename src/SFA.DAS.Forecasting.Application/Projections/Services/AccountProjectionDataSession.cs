using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using SFA.DAS.Forecasting.Data;
using SFA.DAS.Forecasting.Domain.Projections.Services;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Application.Projections.Services
{
    public class AccountProjectionDataSession : IAccountProjectionDataSession
    {
        private readonly IForecastingDataContext _dataContext;

        public AccountProjectionDataSession(IForecastingDataContext dataContext)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            
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
				"INSERT INTO dbo.AccountProjection (" +
				"EmployerAccountId, " +
				"ProjectionCreationDate," +
				"ProjectionGenerationType," +
				"Month," +
				"Year," +
				"FundsIn," +
				"TotalCostOfTraining," +
				"TransferOutTotalCostOfTraining," +
				"TransferInTotalCostOfTraining," +
				"TransferInCompletionPayments," +
				"CompletionPayments," +
				"TransferOutCompletionPayments," +
				"FutureFunds," +
				"CoInvestmentEmployer," +
				"CoInvestmentGovernment" +
				") VALUES ";


			_dataContext.Configuration.AutoDetectChangesEnabled = false;
			insertString.Append(accountCommitmentsInsert);

			foreach (var accountProjectionModel in accountProjections)
			{
				insertString.AppendLine($"({accountProjectionModel.EmployerAccountId}," +
										$"'{accountProjectionModel.ProjectionCreationDate.Date.ToString("yyyy-MM-dd HH:mm:ss")}'," +
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

			await _dataContext.Database.ExecuteSqlCommandAsync(insertString.ToString().Trim().TrimEnd(','));
        }

        public async Task DeleteAll(long employerAccountId)
        {
            await _dataContext.Database.ExecuteSqlCommandAsync(
                "DELETE FROM dbo.AccountProjectionCommitment where AccountProjectionId in (SELECT id from dbo.AccountProjection where EmployerAccountId=@p0)",
                employerAccountId);

            await _dataContext.Database.ExecuteSqlCommandAsync("DELETE FROM dbo.AccountProjection where EmployerAccountId=@p0", employerAccountId);
        }

        public async Task SaveChanges()
        {
            await _dataContext.SaveChangesAsync();
        }

    }
}
