using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using Microsoft.Data.OData.Query.SemanticAst;
using SFA.DAS.Forecasting.Domain.Projections.Services;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;
using IApplicationConfiguration = SFA.DAS.Forecasting.Application.Infrastructure.Configuration.IApplicationConfiguration;

namespace SFA.DAS.Forecasting.Application.Projections.Services
{
    public class AccountProjectionDataService : BaseRepository, IAccountProjectionDataService
    {
        public AccountProjectionDataService(IApplicationConfiguration applicationConfiguration, ILog logger)
            : base(applicationConfiguration.DatabaseConnectionString, logger)
        {
        }
        
        public async Task Store(long employerAccountId, IEnumerable<ReadModel.Projections.AccountProjectionReadModel> accountProjections)
        {
            using (var txScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await WithConnection(async cnn =>
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@employerAccountId", employerAccountId, DbType.Int64);

                    await cnn.ExecuteAsync("Delete From [dbo].[AccountProjection] WHERE EmployerAccountId = @employerAccountId"
                        , parameters, commandType: CommandType.Text);

                    var sql = @"Insert Into [dbo].[AccountProjection] Values 
                           (@employerAccountId,
                           @projectionCreationDate,
                           @projectionGenerationType,
                           @month,
                           @year,
                           @fundsIn,
                           @totalCostOfTraining,
                           @completionPayments,
                           @futureFunds,
                           @coInvestmentEmployer,
                           @coInvestmentGovernment)";

                    foreach (var accountProjectionReadModel in accountProjections)
                    {
                        parameters = new DynamicParameters();
                        parameters.Add("@employerAccountId", employerAccountId, DbType.Int64);
                        parameters.Add("@projectionCreationDate", accountProjectionReadModel.ProjectionCreationDate, DbType.DateTime);
                        parameters.Add("@projectionGenerationType", (short)accountProjectionReadModel.ProjectionGenerationType, DbType.Int16);
                        parameters.Add("@month", accountProjectionReadModel.Month, DbType.Int16);
                        parameters.Add("@year", accountProjectionReadModel.Year, DbType.Int32);
                        parameters.Add("@fundsIn", accountProjectionReadModel.FundsIn, DbType.Decimal, ParameterDirection.Input, null, 10, null);
                        parameters.Add("@totalCostOfTraining", accountProjectionReadModel.TotalCostOfTraining, DbType.Decimal, ParameterDirection.Input, null, 10, null);
                        parameters.Add("@completionPayments", accountProjectionReadModel.CompletionPayments, DbType.Decimal, ParameterDirection.Input, null, 10, null);
                        parameters.Add("@futureFunds", accountProjectionReadModel.FutureFunds, DbType.Decimal, ParameterDirection.Input, null, 10, null);
                        parameters.Add("@coInvestmentEmployer", accountProjectionReadModel.CoInvestmentEmployer, DbType.Decimal, ParameterDirection.Input, null, 10, null);
                        parameters.Add("@coInvestmentGovernment", accountProjectionReadModel.CoInvestmentGovernment, DbType.Decimal, ParameterDirection.Input, null, 10, null);
                        await cnn.ExecuteAsync(sql, parameters, commandType: CommandType.Text);
                    }

                    return 0;
                });
                txScope.Complete();
            }
        }
    }
}
