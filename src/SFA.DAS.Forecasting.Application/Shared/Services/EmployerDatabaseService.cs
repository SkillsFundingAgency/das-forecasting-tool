﻿using Dapper;
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

        Task<List<LevyDeclaration>> GetAccountLevyDeclarations(long accountId, string payrollYear,
            short payrollMonth);
    }

    public class EmployerDatabaseService : BaseRepository, IEmployerDatabaseService
    {
        private readonly ILog _logger;

        public EmployerDatabaseService(
            IApplicationConfiguration config,
            ILog logger)
            : base(config.EmployerConnectionString, logger)
        {
            _logger = logger;
        }

        public async Task<List<LevyDeclaration>> GetAccountLevyDeclarations(long accountId, string payrollYear, short payrollMonth)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);
                parameters.Add("@payrollYear", payrollYear, DbType.String);
                parameters.Add("@payrollMonth", payrollMonth, DbType.Int16);
                var sql = @"Select 
	ldt.Id,
	ldt.AccountId,
	ldt.EmpRef,
	ldt.CreatedDate,
	ldt.SubmissionDate,
	ldt.SubmissionId,
	ldt.PayrollYear,
	ldt.PayrollMonth,
	tl.Amount
    from [employer_financial].[TransactionLine] tl
    join [employer_financial].GetLevyDeclarationAndTopUp ldt on tl.SubmissionId = ldt.SubmissionId
	where tl.AccountId = @accountId 
	and ldt.PayrollMonth = @payrollMonth
	and ldt.PayrollYear = @payrollYear";

                return await c.QueryAsync<LevyDeclaration>(
                    sql,
                    parameters,
                    commandType: CommandType.Text);
            });

            return result.ToList();
        }

        public async Task<IEnumerable<EmployerPayment>> GetEmployerPayments(long accountId, int year, int month)
        {
            const string sql = "SELECT" +
                               "[PaymentId],[Ukprn],[Uln],[AccountId],[ApprenticeshipId] " +
                               ",[CollectionPeriodId],[CollectionPeriodMonth],[CollectionPeriodYear],[DeliveryPeriodMonth],[DeliveryPeriodYear],[Amount] " +
                               ",[ProviderName] ,[StandardCode],[FrameworkCode],[ProgrammeType],[PathwayCode],[PathwayName] " +
                               ",[ApprenticeshipCourseName],[ApprenticeshipCourseStartDate],[ApprenticeshipCourseLevel],[ApprenticeName],[FundingSource] " +
                               "FROM [employer_financial].[Payment] p " +
                               "join [employer_financial].[PaymentMetaData] pmd on p.PaymentMetaDataId = pmd.Id " +
                               "where p.AccountId = @employerAccountId " +
                               "and CollectionPeriodYear = @year " +
                               "and CollectionPeriodMonth = @month";

            try
            {
                return await WithConnection(async cnn =>
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@employerAccountId", accountId, DbType.Int64);
                    parameters.Add("@year", year, DbType.Int32);
                    parameters.Add("@month", month, DbType.Int32);

                    var payments = await cnn.QueryAsync<EmployerPayment>(
                            sql,
                                parameters,
                                commandType: CommandType.Text);
                    return payments.ToList();
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
