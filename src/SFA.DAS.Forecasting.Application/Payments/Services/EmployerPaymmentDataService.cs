using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.Forecasting.Domain.Payments.Services;
using SFA.DAS.Forecasting.Models.Payments;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.Forecasting.Application.Payments.Services
{
	public class EmployerPaymmentDataService : BaseRepository, IEmployerPaymentDataService
	{
		public EmployerPaymmentDataService(string connectionString, ILog logger) : base(connectionString, logger)
		{
		}

		public async Task StoreEmployerPayment(Payment employerPayment)
		{
			await WithTransaction(async (cnn, tx) =>
			{
				try
				{
					await StoreEmployerPayment(cnn, employerPayment);
					tx.Commit();
				}
				catch (Exception e)
				{
					tx.Rollback();
					throw;
				}
			});
		}

		public async Task<List<Payment>> GetEmployerPayments(long employerAccountId, int month, int year)
		{
			return await WithConnection(async cnn =>
			{
				var parameters = new DynamicParameters();
				parameters.Add("@employerAccountId", employerAccountId, DbType.Int64);
				parameters.Add("@collectionPeriodYear", year, DbType.String);
				parameters.Add("@collectionPeriodMonth", month, DbType.Byte);

				var employerPayments = await cnn.QueryAsync<Payment>(
					sql:
					"SELECT Id, ExternalPaymentId, EmployerAccountId, ProviderId, ApprenticeshipId, Amount, LearnerId, CollectionPeriodMonth, CollectionPeriodYear FROM [dbo].[Payment] WHERE EmployerAccountId = @employerAccountId and CollectionPeriodYear = @collectionPeriodYear and CollectionPeriodMonth = @collectionPeriodMonth",
					param: parameters,
					commandType: CommandType.Text);
				return employerPayments.ToList();
			});
		}

		private async Task StoreEmployerPayment(IDbConnection connection, Payment employerPayment)
		{
			var parameters = new DynamicParameters();
			parameters.Add("@id", employerPayment.Id, DbType.Int64);
			parameters.Add("@externalPaymentId", employerPayment.ExternalPaymentId, DbType.String);
			parameters.Add("@employerAccountId", employerPayment.EmployerAccountId, DbType.Int64);
			parameters.Add("@providerId", employerPayment.ProviderId, DbType.Int64);
			parameters.Add("@apprenticeshipId", employerPayment.ApprenticeshipId, DbType.Int64);
			parameters.Add("@amount", employerPayment.Amount, DbType.Decimal);
			parameters.Add("@learnerId", employerPayment.LearnerId, DbType.Int64);
			parameters.Add("@receivedTime", employerPayment.ReceivedTime, DbType.DateTime);
			parameters.Add("@collectionPeriodMonth", employerPayment.CollectionPeriod.Month, DbType.Int32);
			parameters.Add("@collectionPeriodYear", employerPayment.CollectionPeriod.Year, DbType.Int32);

			await connection.ExecuteAsync(
				@"MERGE Payment AS target 
                                    USING(SELECT @id, @externalPaymentId, @employerAccountId, @providerId, @apprenticeshipId, @amount, @learnerId, @collectionPeriodMonth, @collectionPeriodYear) AS source(Id, ExternalPaymentId, EmployerAccountId, ProviderId, ApprenticeshipId, Amount, LearnerId, CollectionPeriodMonth, CollectionPeriodYear)
                                    ON(target.EmployerAccountId = source.EmployerAccountId and target.ExternalPaymentId = source.ExternalPaymentId and target.ProviderId = source.ProviderId and target.LearnerId = source.LearnerId and target.LearnerId = source.LearnerId and target.CollectionPeriodMonth = source.CollectionPeriodMonth and target.CollectionPeriodYear = source.CollectionPeriodYear)
                                    WHEN MATCHED THEN
                                        UPDATE SET Amount = source.Amount, ReceivedTime = source.ReceivedTime
                                    WHEN NOT MATCHED THEN
                                        INSERT(Id, ExternalPaymentId, EmployerAccountId, ProviderId, ApprenticeshipId, Amount, LearnerId, CollectionPeriodMonth, CollectionPeriodYear)
                                        VALUES(source.Id, source.ExternalPaymentId, source.EmployerAccountId, source.ProviderId, source.ApprenticeshipId, source.Amount, source.LearnerId, source.CollectionPeriodMonth, source.CollectionPeriodYear);",
				parameters,
				commandType: CommandType.Text);
		}
	}
}
