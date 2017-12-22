using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using SFA.DAS.Forecasting.Domain.Entities;
using SFA.DAS.Forecasting.Domain.Interfaces;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.Forecasting.Infrastructure.Repositories
{
    public class BalanceRepository : BaseRepository, IBalanceRepository
    {
        public BalanceRepository(IConfiguration configuration, ILog logger)
            : base(configuration.DatabaseConnectionString, logger)
        {
        }

        public async Task<EmployerBalance> GetEmployerBalanceAndLevyAsync(long employerId)
        {
            return await WithConnection(
                async c =>
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@employerAccountId", employerId, DbType.Int64);

                    var result =
                        await
                        c.QueryAsync<EmployerBalance>(
                              sql: "SELECT * FROM [dbo].[CurrentBalanceAndLevy] WHERE EmployerAccountId = @employerAccountId"
                            , param: parameters,
                              commandType: CommandType.Text);

                    return result.Single();
                });
        }

        public async Task<IEnumerable<BalanceItem>> GetBalanceAsync(long employerId)
        {
            var  data = await GetEmployerBalanceAndLevyAsync(employerId);
            return GetData(data.Balance, data.LevyCredit);
            
        }

        private IEnumerable<BalanceItem> GetData(decimal balance, decimal credit)
        {
            for (int i = 0; i < 20; i++)
            {
                var m = DateTime.Now.AddMonths(i);
                yield return new BalanceItem
                {
                    Date = m,
                    LevyCredit = (int)credit,
                    CostOfTraining = 100,
                    CompletionPayments = 0,
                    ExpiredFunds = 0,
                    Balance = (700 * (i + 1)) + (int)balance
                };
            }
        }


    }
}
