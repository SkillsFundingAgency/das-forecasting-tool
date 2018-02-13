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
                           @totalCostOfTraning,
                           @completionPayments,
                           @futureFunds)";

                    foreach (var accountProjectionReadModel in accountProjections)
                    {
                        parameters = new DynamicParameters();
                        parameters.Add("@employerAccountId", employerAccountId, DbType.Int64);
                        parameters.Add("@projectionCreationDate", accountProjectionReadModel.ProjectionCreationDate, DbType.DateTime);
                        parameters.Add("@projectionGenerationType", (short)accountProjectionReadModel.ProjectionGenerationType, DbType.Int16);
                        parameters.Add("@month", accountProjectionReadModel.Month, DbType.Int16);
                        parameters.Add("@year", accountProjectionReadModel.Year, DbType.Int32);
                        parameters.Add("@fundsIn", accountProjectionReadModel.FundsIn, DbType.Decimal);
                        parameters.Add("@totalCostOfTraning", accountProjectionReadModel.TotalCostOfTraining, DbType.Decimal);
                        parameters.Add("@completionPayments", accountProjectionReadModel.CompletionPayments, DbType.Decimal);
                        parameters.Add("@futureFunds", accountProjectionReadModel.FutureFunds, DbType.Decimal);
                        await cnn.ExecuteAsync(sql, parameters, commandType: CommandType.Text);
                    }

                    return 0;
                });
                txScope.Complete();
            }
        }
    }
}
