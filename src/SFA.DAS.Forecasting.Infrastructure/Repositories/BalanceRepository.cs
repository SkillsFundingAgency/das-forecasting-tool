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
        public BalanceRepository(IApplicationConfiguration applicationConfiguration, ILog logger)
            : base(applicationConfiguration.DatabaseConnectionString, logger)
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
            // ToDo: Move mapping to handler
            // ToDo: Remove test data
            if(employerId == 12346)
                return GetData();

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

        private IEnumerable<BalanceItem> GetData()
        {
            var startMonth = DateTime.Now;
            return new List<BalanceItem>
                           {
                               new BalanceItem { Date = startMonth.AddMonths(0), Balance = 980 },
                               new BalanceItem { Date = startMonth.AddMonths(1), Balance = 50 },
                               new BalanceItem { Date = startMonth.AddMonths(2), Balance = -100 },
                               new BalanceItem { Date = startMonth.AddMonths(3), Balance = 53 },
                               new BalanceItem { Date = startMonth.AddMonths(4), Balance = 12 },
                               new BalanceItem { Date = startMonth.AddMonths(5), Balance = 978 },
                               new BalanceItem { Date = startMonth.AddMonths(5), Balance = 976 },
                               new BalanceItem { Date = startMonth.AddMonths(7), Balance = 925 },
                               new BalanceItem { Date = startMonth.AddMonths(8), Balance = 750 },
                               new BalanceItem { Date = startMonth.AddMonths(9), Balance = 450 },
                               new BalanceItem { Date = startMonth.AddMonths(10), Balance = 325 },
                               new BalanceItem { Date = startMonth.AddMonths(11), Balance = 300 },
                               new BalanceItem { Date = startMonth.AddMonths(12), Balance = 198 },
                               new BalanceItem { Date = startMonth.AddMonths(13), Balance = 50 },
                               new BalanceItem { Date = startMonth.AddMonths(14), Balance = -10 },
                               new BalanceItem { Date = startMonth.AddMonths(15), Balance = 53 },
                               new BalanceItem { Date = startMonth.AddMonths(16), Balance = 12 },
                               new BalanceItem { Date = startMonth.AddMonths(17), Balance = 978 },
                               new BalanceItem { Date = startMonth.AddMonths(18), Balance = 976 },
                               new BalanceItem { Date = startMonth.AddMonths(19), Balance = 925 },
                               new BalanceItem { Date = startMonth.AddMonths(20), Balance = 750 },
                               new BalanceItem { Date = startMonth.AddMonths(21), Balance = 450 },
                               new BalanceItem { Date = startMonth.AddMonths(22), Balance = 325 },
                               new BalanceItem { Date = startMonth.AddMonths(23), Balance = 300 },
                               new BalanceItem { Date = startMonth.AddMonths(24), Balance = 15 },
                               new BalanceItem { Date = startMonth.AddMonths(25), Balance = 30 },
                               new BalanceItem { Date = startMonth.AddMonths(26), Balance = -25 },
                               new BalanceItem { Date = startMonth.AddMonths(27), Balance = 1 },
                               new BalanceItem { Date = startMonth.AddMonths(28), Balance = -10 },
                               new BalanceItem { Date = startMonth.AddMonths(29), Balance = 50 },
                               new BalanceItem { Date = startMonth.AddMonths(30), Balance = 505 },
                               new BalanceItem { Date = startMonth.AddMonths(31), Balance = 543 },
                               new BalanceItem { Date = startMonth.AddMonths(32), Balance = 234 },
                               new BalanceItem { Date = startMonth.AddMonths(33), Balance = 725 },
                               new BalanceItem { Date = startMonth.AddMonths(34), Balance = 75 },
                               new BalanceItem { Date = startMonth.AddMonths(35), Balance = 555 },
                               new BalanceItem { Date = startMonth.AddMonths(36), Balance = 150 },
                               new BalanceItem { Date = startMonth.AddMonths(37), Balance = 300 },
                               new BalanceItem { Date = startMonth.AddMonths(38), Balance = -243 },
                               new BalanceItem { Date = startMonth.AddMonths(39), Balance = 188 },
                               new BalanceItem { Date = startMonth.AddMonths(40), Balance = 43 },
                               new BalanceItem { Date = startMonth.AddMonths(41), Balance = 675 },
                               new BalanceItem { Date = startMonth.AddMonths(42), Balance = -135 },
                               new BalanceItem { Date = startMonth.AddMonths(43), Balance = 654 },
                               new BalanceItem { Date = startMonth.AddMonths(44), Balance = 345 },
                               new BalanceItem { Date = startMonth.AddMonths(45), Balance = -567 },
                               new BalanceItem { Date = startMonth.AddMonths(46), Balance = 35 },
                               new BalanceItem { Date = startMonth.AddMonths(47), Balance = 671 }
                           };
        }


    }
}
