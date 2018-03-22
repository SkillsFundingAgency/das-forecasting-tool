using Dapper;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Models.Payments;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.Forecasting.Application.Shared.Services
{
    public interface IEmployerDatabaseService
    {
        Task<IEnumerable<EmployerPayment>> GetEmployerPayments(long accountId, int year, int month);

        Task<List<LevyDeclarationView>> GetAccountLevyDeclarations(long accountId, string payrollYear,
            short payrollMonth);
    }

    public class EmployerDatabaseService : BaseRepository, IEmployerDatabaseService
    {
        private ILog _logger;

        public EmployerDatabaseService(
            IApplicationConfiguration config,
            ILog logger)
            : base(config.EmployerConnectionString, logger)
        {
            _logger = logger;
        }

        public async Task<List<LevyDeclarationView>> GetAccountLevyDeclarations(long accountId, string payrollYear, short payrollMonth)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);
                parameters.Add("@payrollYear", payrollYear, DbType.String);
                parameters.Add("@payrollMonth", payrollMonth, DbType.Int16);
                var sql = @"SELECT *
FROM [employer_financial].[GetLevyDeclarationAndTopUp] x
WHERE x.AccountId = @accountId
AND x.PayrollYear = @payrollYear
AND x.PayrollMonth = @payrollMonth";

                return await c.QueryAsync<LevyDeclarationView>(
                    sql,
                    parameters,
                    commandType: CommandType.Text);
            });

            return result.ToList();
        }

        public async Task<IEnumerable<EmployerPayment>> GetEmployerPayments(long accountId, int year, int month)
        {
            // Get all Payments where AccountId and  ,CollectionPeriodMonth ,CollectionPeriodYear --> 10005694, 5, 2017
            var sql = "SELECT" +
                        "[PaymentId],[Ukprn],[Uln],[AccountId],[ApprenticeshipId] " +
                        ",[CollectionPeriodId],[CollectionPeriodMonth],[CollectionPeriodYear],[Amount],[PaymentMetaDataId],[ProviderName] " +
                        ",[StandardCode],[FrameworkCode],[ProgrammeType],[PathwayCode],[PathwayName] " +
                        ",[ApprenticeshipCourseName],[ApprenticeshipCourseStartDate],[ApprenticeshipCourseLevel],[ApprenticeName],[FundingSource]" +
                    "FROM [employer_financial].[Payment] " +
                    "inner join [employer_financial].[PaymentMetaData] metaData " +
                    "on payment.PaymentMetaDataId = metaData.Id " +
                    "where AccountId = @employerAccountId " + 
                    "and CollectionPeriodYear = @year " +  
                    "and CollectionPeriodMonth = @month ";

            try
            {
                return await WithConnection(async cnn =>
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@employerAccountId", accountId, DbType.Int64);
                    parameters.Add("@year", year, DbType.Int32);
                    parameters.Add("@month", month, DbType.Int32);

                    var levyDeclarations = await cnn.QueryAsync<EmployerPayment>(
                            sql,
                                parameters,
                                commandType: CommandType.Text);
                    return levyDeclarations.ToList();
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get employer payments");
                throw;
            }
        }
    }
}
