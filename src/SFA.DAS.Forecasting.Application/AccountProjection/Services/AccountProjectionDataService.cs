using Dapper;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.AccountProjection;
using IApplicationConfiguration = SFA.DAS.Forecasting.Application.Infrastructure.Configuration.IApplicationConfiguration;

namespace SFA.DAS.Forecasting.Application.AccountProjection.Services
{
    public class AccountProjectionDataService : BaseRepository, IAccountProjectionDataService
    {
        public AccountProjectionDataService(IApplicationConfiguration applicationConfiguration, ILog logger)
            : base(applicationConfiguration.DatabaseConnectionString, logger)
        {
        }

        public async Task<IEnumerable<ReadModel.AccountProjections.AccountProjection>> Get(long employerId)
        {
            return await WithConnection(
               async c =>
               {
                   var parameters = new DynamicParameters();
                   parameters.Add("@employerAccountId", employerId, DbType.Int64);

                   var result =
                       await
                       c.QueryAsync<ReadModel.AccountProjections.AccountProjection>(
                             sql: "SELECT * FROM [dbo].[AccountProjection] WHERE EmployerAccountId = @employerAccountId"
                           , param: parameters,
                             commandType: CommandType.Text);

                   return result;
               });
        }
    }
}
