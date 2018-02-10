using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
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
            await WithTransaction(async (cnn, tx) =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@employerAccountId", employerAccountId, DbType.Int64);

                await cnn.ExecuteAsync("Delete From [dbo].[AccountProjection] WHERE EmployerAccountId = @employerAccountId"
                            , parameters, commandType: CommandType.Text);

                var sql = @"Insert INTO [dbo].[AccountProjection] Values 
                           (@EmployerAccountId,
                           @ProjectionCreationDate,
                           @ProjectionGenerationType,
                           @Month,
                           @Year,
                           @FundsIn,
                           @TotalCostOfTraning,
                           @CompletionPayments,
                           @FutureFunds)";
                await cnn.ExecuteAsync(sql, accountProjections);
            });
        }
    }
}
