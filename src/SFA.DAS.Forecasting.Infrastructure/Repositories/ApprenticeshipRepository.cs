using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Dapper;

using SFA.DAS.Forecasting.Domain.Entities;
using SFA.DAS.Forecasting.Domain.Interfaces;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.Forecasting.Infrastructure.Repositories
{
    public class ApprenticeshipRepository : BaseRepository, IApprenticeshipRepository
    {
        public ApprenticeshipRepository(string connectionString, ILog logger)
            : base(connectionString, logger)
        {
        }

        public async Task<IEnumerable<Apprenticeship>> GetApprenticeships(long employerId)
        {
            return await WithConnection(
                async c =>
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("@employerAccountId", employerId, DbType.Int64);

                        return
                            await
                            c.QueryAsync<Apprenticeship>(
                                  sql: "SELECT * FROM Apprenticeship WHERE EmployerAccountId = @employerAccountId"
                                , param: parameters, 
                                  commandType: CommandType.Text);
                    });
        }
    }
}