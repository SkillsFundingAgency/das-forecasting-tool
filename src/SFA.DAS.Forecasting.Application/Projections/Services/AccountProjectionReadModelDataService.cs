using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.Forecasting.Application.Projections.Services
{
    public interface IAccountProjectionReadModelDataService
    {
        Task<IEnumerable<Models.Projections.AccountProjectionReadModel>> Get(long employerId);
    }

    public class AccountProjectionReadModelDataService : BaseRepository, IAccountProjectionReadModelDataService
    {
        public AccountProjectionReadModelDataService(IApplicationConfiguration applicationConfiguration, ILog logger)
            : base(applicationConfiguration.DatabaseConnectionString, logger)
        {
        }

        public async Task<IEnumerable<Models.Projections.AccountProjectionReadModel>> Get(long employerId)
        {
            return await WithConnection(
                async c =>
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@employerAccountId", employerId, DbType.Int64);

                    var result = await c.QueryAsync<Models.Projections.AccountProjectionReadModel>(
                                "SELECT * FROM [dbo].[AccountProjection] WHERE EmployerAccountId = @employerAccountId"
                                ,parameters ,
                                commandType: CommandType.Text);

                    return result;
                });
        }

    }
}