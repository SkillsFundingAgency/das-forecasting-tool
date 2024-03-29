﻿using SFA.DAS.Forecasting.Models.Payments;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;
using Dapper;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Converters;
using SFA.DAS.Forecasting.Core.Configuration;
using SFA.DAS.Forecasting.Data;

namespace SFA.DAS.Forecasting.Application.Shared.Services
{
    public interface IEmployerDatabaseService
    {
        Task<List<EmployerPayment>> GetEmployerPayments(long accountId, int year, int month);

        Task<List<LevyDeclaration>> GetAccountLevyDeclarations(long accountId, string payrollYear,
            short payrollMonth);

	    Task<List<PeriodInformation>> GetPeriodIds();

		Task<List<long>> GetEmployersWithPayments(int year, int month);

        Task<IList<long>> GetAccountIdsForPeriod(string payrollYear, short payrollMonth);

        Task<List<EmployerPayment>> GetPastEmployerPayments(long accountId, int year, int month);
        Task<IList<long>> GetAccountIds();
    }

    public class EmployerDatabaseService : BaseRepository, IEmployerDatabaseService
    {
        private readonly ILogger<EmployerDatabaseService> _logger;

        public EmployerDatabaseService(
            ForecastingJobsConfiguration config,
            ILogger<EmployerDatabaseService>  logger, ChainedTokenCredential chainedTokenCredential) :base(config.EmployerConnectionString, s => {}, chainedTokenCredential)
            
        {
            _logger = logger;
        }
        public async Task<IList<long>> GetAccountIds()
        {
            var result = await WithConnection(async c =>
            {
                var sql = @"Select distinct
	                    tl.AccountId
                        from [employer_financial].[TransactionLine] tl";

                return await c.QueryAsync<long>(
                    sql,
                    commandType: CommandType.Text);
            });

            return result.ToList();
        }

        public async Task<IList<long>> GetAccountIdsForPeriod(string payrollYear, short payrollMonth)
		{
			var result = await WithConnection(async c =>
			{
				var parameters = new DynamicParameters();
				parameters.Add("@payrollYear", payrollYear, DbType.String);
				parameters.Add("@payrollMonth", payrollMonth, DbType.Int16);
				var sql = @"Select distinct
	                    tl.AccountId
                        from [employer_financial].[TransactionLine] tl";

				return await c.QueryAsync<long>(
					sql,
					commandType: CommandType.Text);
			});

			return result.ToList();
		}

	    public async Task<List<PeriodInformation>> GetPeriodIds()
	    {
		    var result = await WithConnection(async c =>
		    {
			    var sql = @"SELECT TOP (1000) [PeriodEndId]
							  ,[CalendarPeriodMonth]
							  ,[CalendarPeriodYear]
						  FROM [employer_financial].[PeriodEnd]";

			    return await c.QueryAsync<PeriodInformation>(
				    sql,
				    commandType: CommandType.Text);
		    });

		    return result.ToList();
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
                        ldt.SubmissionId,
	                    ldt.AccountId,
	                    ldt.EmpRef,
	                    ldt.CreatedDate,
	                    ldt.SubmissionDate,
	                    ldt.PayrollYear,
	                    ldt.PayrollMonth,
	                    tl.Amount Amount
                        from [employer_financial].[TransactionLine] tl
                        join [employer_financial].LevyDeclaration ldt on tl.SubmissionId = ldt.SubmissionId
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

        public async Task<List<EmployerPayment>> GetEmployerPayments(long accountId, int year, int month)
        {
            const string sql = @"SELECT
                               [PaymentId], [Ukprn], [Uln], [AccountId], p.[ApprenticeshipId] 
                               ,[CollectionPeriodId],[CollectionPeriodMonth],[CollectionPeriodYear],[DeliveryPeriodMonth],[DeliveryPeriodYear],p.[Amount] 
                               ,[ProviderName] ,[StandardCode],[FrameworkCode],[ProgrammeType],[PathwayCode],[PathwayName] 
                               ,[ApprenticeshipCourseName],[ApprenticeshipCourseStartDate],[ApprenticeshipCourseLevel],[ApprenticeName], [FundingSource], acct.[SenderAccountId] 
                               from [employer_financial].[Payment] p 
                               left join [employer_financial].[Accounttransfers] acct on p.AccountId = acct.ReceiverAccountId and p.ApprenticeshipId = acct.ApprenticeshipId and p.PeriodEnd = acct.PeriodEnd 
                               join [employer_financial].[PaymentMetaData] pmd on p.PaymentMetaDataId = pmd.Id 
                               where p.AccountId = @employerAccountId 
                               and CollectionPeriodYear = @year 
                               and CollectionPeriodMonth = @month";

            try
            {
                return await WithConnection(async cnn =>
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@employerAccountId", accountId, DbType.Int64);
                    parameters.Add("@year", year, DbType.Int32);
                    parameters.Add("@month", month, DbType.Int32);

                    var payments = (await cnn.QueryAsync<EmployerPayment>(
                            sql,
                                parameters,
                                commandType: CommandType.Text)).ToList();
                    payments.ForEach(payment => payment.FundingSource = (int)payment.FundingSource == (int)FundingSourceConverter.ConvertToApiFundingSource(FundingSource.Transfer) ? FundingSourceConverter.ConvertToApiFundingSource(FundingSource.Transfer) : payment.FundingSource);
                    return payments;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get employer payments");
                throw;
            }
        }

        public async Task<List<EmployerPayment>> GetPastEmployerPayments(long accountId, int year, int month)
        {
            const string sql = @"SELECT
	                            [PaymentId], 
	                            tl.AccountId,
                                tl.TransactionDate,
	                            acct.[SenderAccountId] ,
	                            tl.Ukprn,
	                            p.[ApprenticeshipId] ,
	                            p.Amount,
	                            [Uln],  
	                            [CollectionPeriodId],
	                            [CollectionPeriodMonth],[CollectionPeriodYear],
	                            [DeliveryPeriodMonth],
	                            [DeliveryPeriodYear]
	                            ,[ProviderName] ,[StandardCode],[FrameworkCode],[ProgrammeType],[PathwayCode],[PathwayName] 
	                            ,[ApprenticeshipCourseName],[ApprenticeshipCourseStartDate],[ApprenticeshipCourseLevel],[ApprenticeName], [FundingSource], acct.[SenderAccountId] 
                            from [employer_financial].[Payment] p 
                            left join [employer_financial].[Accounttransfers] acct on p.AccountId = acct.ReceiverAccountId and p.ApprenticeshipId = acct.ApprenticeshipId and p.PeriodEnd = acct.PeriodEnd 
                            inner join [employer_financial].[PaymentMetaData] pmd on p.PaymentMetaDataId = pmd.Id 
                            inner join [employer_financial].TransactionLine tl on tl.PeriodEnd = p.PeriodEnd and tl.AccountId = p.AccountId and tl.Ukprn = p.Ukprn
                             where p.AccountId = @employerAccountId 
                               and CollectionPeriodYear = @year 
                               and CollectionPeriodMonth = @month";

            try
            {
                return await WithConnection(async cnn =>
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@employerAccountId", accountId, DbType.Int64);
                    parameters.Add("@year", year, DbType.Int32);
                    parameters.Add("@month", month, DbType.Int32);

                    var payments = (await cnn.QueryAsync<EmployerPayment>(
                        sql,
                        parameters,
                        commandType: CommandType.Text)).ToList();
                    payments.ForEach(payment => payment.FundingSource = (int)payment.FundingSource == (int)FundingSourceConverter.ConvertToApiFundingSource(FundingSource.Transfer) ? FundingSourceConverter.ConvertToApiFundingSource(FundingSource.Transfer) : payment.FundingSource);
                    return payments;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get past employer payments");
                throw;
            }
        }

        public async Task<List<long>> GetEmployersWithPayments(int year, int month)
        {
            const string sql = "SELECT distinct" +
                               "[AccountId]" +
                               "from [employer_financial].[Payment] p " +
                               "left join [employer_financial].[Accounttransfers] acct on p.AccountId = acct.ReceiverAccountId and p.ApprenticeshipId = acct.ApprenticeshipId and p.PeriodEnd = acct.PeriodEnd " +
                               "inner join [employer_financial].[PaymentMetaData] pmd on p.PaymentMetaDataId = pmd.Id " +
                               "and CollectionPeriodYear = @year " +
                               "and CollectionPeriodMonth = @month";

            try
            {
                return await WithConnection(async cnn =>
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@year", year, DbType.Int32);
                    parameters.Add("@month", month, DbType.Int32);

                    var payments = (await cnn.QueryAsync<long>(
                            sql,
                                parameters,
                                commandType: CommandType.Text)).ToList();
                    return payments;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get employers for year: {year} and month {month}");
                throw;
            }
        }
    }
}
