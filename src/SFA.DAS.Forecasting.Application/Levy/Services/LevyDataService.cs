﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Domain.Levy.Model;
using SFA.DAS.Forecasting.Domain.Levy.Services;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.Forecasting.Application.Levy.Services
{
    public class LevyDataService : BaseRepository, ILevyDataService
    {
        public LevyDataService(IApplicationConfiguration applicationConfiguration, ILog log) : base(applicationConfiguration.DatabaseConnectionString, log)
        {
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
                            sql: "SELECT Id, EmployerAccountId, Scheme, PayrollYear, PayrollMonth, LevyAmountDeclared, TransactionDate, DateReceived FROM [dbo].[LevyDeclaration] WHERE EmployerAccountId = @employerAccountId and PayrollYearStart = @payrollYearStart and PayrollMonth = @payrollMonth",
                            param: parameters,
                            commandType: CommandType.Text);
                return levyDeclarations.ToList();
            });
        }

        public async Task StoreLevyDeclarations(IEnumerable<LevyDeclaration> levyDeclarations)
        {
            await WithTransaction(async (cnn, tx) =>
            {
                try
                {
                    foreach (var levyDeclaration in levyDeclarations)
                        await StoreLevyDeclaration(cnn, levyDeclaration);
                    tx.Commit();
                }
                catch (Exception e)
                {
                    tx.Rollback();
                    throw;
                }
            });
        }

        public async Task<decimal> GetLatestLevyAmount(long employerAccountId)
        {
            return await WithConnection(async cnn =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@employerAccountId", employerAccountId, DbType.Int64);

                return await cnn.ExecuteScalarAsync<decimal>(
                    sql: "SELECT Id, EmployerAccountId, Scheme, PayrollYear, PayrollMonth, LevyAmountDeclared, TransactionDate, DateReceived FROM [dbo].[LevyDeclaration] WHERE EmployerAccountId = @employerAccountId and PayrollYearStart = @payrollYearStart and PayrollMonth = @payrollMonth",
                    param: parameters,
                    commandType: CommandType.Text);
            });
        }

        private async Task StoreLevyDeclaration(IDbConnection connection, LevyDeclaration levyDeclaration)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@employerAccountId", levyDeclaration.EmployerAccountId, DbType.Int64);
            parameters.Add("@payrollYear", levyDeclaration.PayrollYear, DbType.String);
            parameters.Add("@payrollMonth", levyDeclaration.PayrollMonth, DbType.Byte);
            parameters.Add("@levyAmountDeclared", levyDeclaration.LevyAmountDeclared, DbType.Decimal);
            parameters.Add("@transactionDate", levyDeclaration.TransactionDate, DbType.Byte);

            await connection.ExecuteAsync(
                @"MERGE LevyDeclaration AS target 
                                    USING(SELECT @employerAccountId, @scheme, @payrollYear, @payrollMonth, @levyAmountDeclared, @transactionDate) AS source(EmployerAccountId, Scheme, PayrollYear, PayrollMonth, LevyAmountDeclared, TransactionDate)
                                    ON(target.EmployerAccountId = source.EmployerAccountId and target.Scheme = source.Scheme and target.PayrollYear = source.PayrollYear and target.PayrollMonth = source.PayrollMonth)
                                    WHEN MATCHED THEN
                                        UPDATE SET LevyAmountDeclared = source.LevyAmountDeclared, DateReceived = getdate()
                                    WHEN NOT MATCHED THEN
                                        INSERT(EmployerAccountId, Scheme, PayrollYear, PayrollMonth, LevyAmountDeclared, TransactionDate)
                                        VALUES(source.EmployerAccountId, source.Scheme, source.PayrollYear, source.PayrollMonth, source.LevyAmountDeclared, source.TransactionDate);",
                parameters,
                commandType: CommandType.Text);
        }
    }
}
