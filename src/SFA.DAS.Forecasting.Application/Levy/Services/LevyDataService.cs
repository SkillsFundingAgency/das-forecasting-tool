using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Domain.Levy.Services;
using SFA.DAS.Forecasting.Models.Levy;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.Forecasting.Application.Levy.Services
{
    public class LevyDataService : BaseRepository, ILevyDataService
    {
        public ILog Logger { get; }

        public LevyDataService(IApplicationConfiguration applicationConfiguration, ILog log) : base(applicationConfiguration.DatabaseConnectionString, log)
        {
            Logger = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task<List<LevyDeclaration>> GetLevyDeclarationsForPeriod(long employerAccountId, string payrollYear, byte payrollMonth)
        {
            return await WithConnection(async cnn =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@employerAccountId", employerAccountId, DbType.Int64);
                parameters.Add("@payrollYear", payrollYear, DbType.String);
                parameters.Add("@payrollMonth", payrollMonth, DbType.Byte);

                var levyDeclarations = await cnn.QueryAsync<LevyDeclaration>(
                            "SELECT Id, EmployerAccountId, Scheme, PayrollYear, PayrollMonth, PayrollDate, LevyAmountDeclared, TransactionDate, DateReceived FROM [dbo].[LevyDeclaration] WHERE EmployerAccountId = @employerAccountId and PayrollYear = @payrollYear and PayrollMonth = @payrollMonth",
                            parameters,
                            commandType: CommandType.Text);
                return levyDeclarations.ToList();
            });
        }

        public async Task StoreLevyDeclarations(IEnumerable<LevyDeclaration> levyDeclarations)
        {
            var txScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                try
                {
                    await WithConnection(async cnn =>
                    {
                        foreach (var levyDeclaration in levyDeclarations)
                        {
                            await StoreLevyDeclaration(cnn, levyDeclaration);
                        }

                        return 0;
                    });
                    txScope.Complete();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"Error storing levy declarations. Error: {ex}");
                    throw;
                }
            }
            finally
            {
                txScope.Dispose();
            }
        }

        public async Task<decimal> GetLatestLevyAmount(long employerAccountId)
        {
            return await WithConnection(async cnn =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@employerAccountId", employerAccountId, DbType.Int64);

                return await cnn.ExecuteScalarAsync<decimal>(
                    sql: @"select 
	top 1 
	sum(LevyAmountDeclared) 
	from LevyDeclaration	
    where EmployerAccountId = @employerAccountId
	group by PayrollDate 
	order by PayrollDate desc",
                    param: parameters,
                    commandType: CommandType.Text);
            });
        }

        private async Task StoreLevyDeclaration(IDbConnection connection, LevyDeclaration levyDeclaration)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@employerAccountId", levyDeclaration.EmployerAccountId, DbType.Int64);
            parameters.Add("@scheme", levyDeclaration.Scheme, DbType.String);
            parameters.Add("@payrollYear", levyDeclaration.PayrollYear, DbType.String);
            parameters.Add("@payrollMonth", levyDeclaration.PayrollMonth, DbType.Byte);
            parameters.Add("@payrollDate", levyDeclaration.PayrollDate, DbType.DateTime);
            parameters.Add("@levyAmountDeclared", levyDeclaration.LevyAmountDeclared, DbType.Decimal, ParameterDirection.Input, null, 18, 2);
            parameters.Add("@transactionDate", levyDeclaration.TransactionDate, DbType.DateTime);

            await connection.ExecuteAsync(
                @"MERGE LevyDeclaration AS target 
                                    USING(SELECT @employerAccountId, @scheme, @payrollYear, @payrollMonth, @payrollDate, @levyAmountDeclared, @transactionDate) AS source(EmployerAccountId, Scheme, PayrollYear, PayrollMonth, PayrollDate, LevyAmountDeclared, TransactionDate)
                                    ON(target.EmployerAccountId = source.EmployerAccountId and target.Scheme = source.Scheme and target.PayrollYear = source.PayrollYear and target.PayrollMonth = source.PayrollMonth)
                                    WHEN MATCHED THEN
                                        UPDATE SET LevyAmountDeclared = source.LevyAmountDeclared, DateReceived = getdate()
                                    WHEN NOT MATCHED THEN
                                        INSERT(EmployerAccountId, Scheme, PayrollYear, PayrollMonth, PayrollDate, LevyAmountDeclared, TransactionDate)
                                        VALUES(source.EmployerAccountId, source.Scheme, source.PayrollYear, source.PayrollMonth, source.PayrollDate, source.LevyAmountDeclared, source.TransactionDate);",
                parameters,
                commandType: CommandType.Text);
        }
    }
}
