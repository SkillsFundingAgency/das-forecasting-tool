using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
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

        public async Task<IEnumerable<ReadModel.Projections.AccountProjection>> Get(long employerId)
        {
            return await WithConnection(
               async c =>
               {
                   var parameters = new DynamicParameters();
                   parameters.Add("@employerAccountId", employerId, DbType.Int64);

                   var result =
                       await
                       c.QueryAsync<ReadModel.Projections.AccountProjection>(
                             sql: "SELECT * FROM [dbo].[AccountProjection] WHERE EmployerAccountId = @employerAccountId"
                           , param: parameters,
                             commandType: CommandType.Text);

                   return result;
               });
        }

        public async Task Store(IEnumerable<Domain.Projections.AccountProjection> accountProjections)
        {
            throw new System.NotImplementedException();
        }
    }
}
