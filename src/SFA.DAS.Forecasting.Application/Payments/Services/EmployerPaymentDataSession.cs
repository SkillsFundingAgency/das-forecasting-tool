using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Data;
using SFA.DAS.Forecasting.Domain.Payments.Services;
using SFA.DAS.Forecasting.Models.Payments;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CalendarPeriod = SFA.DAS.EmployerFinance.Types.Models.CalendarPeriod;

namespace SFA.DAS.Forecasting.Application.Payments.Services
{
    public class EmployerPaymentDataSession : IEmployerPaymentDataSession
    {
        private readonly IForecastingDataContext _dataContext;
        private readonly ITelemetry _telemetry;

        public EmployerPaymentDataSession(IForecastingDataContext dataContext, ITelemetry telemetry)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public async Task<PaymentModel> Get(long employerAccountId, string paymentId)
        {
            var stopwatch = new Stopwatch();
            var startTime = DateTime.UtcNow;
            stopwatch.Start();
            var employerPayment = await _dataContext.Payments
                .FirstOrDefaultAsync(payment => payment.EmployerAccountId == employerAccountId &&
                                                payment.ExternalPaymentId == paymentId);
            stopwatch.Stop();
            _telemetry.TrackDependency(DependencyType.SqlDatabaseQuery, "Get Payment", startTime, stopwatch.Elapsed, employerPayment != null);
            return employerPayment;
        }

        public void Store(PaymentModel payment)
        {
            if (payment.Id <= 0)
                _dataContext.Payments.Add(payment);
        }

        public async Task<DateTime?> GetLastReceivedTime(long employerAccountId)
        {
            return await _dataContext
                .Payments.Where(payment => payment.EmployerAccountId == employerAccountId || payment.SendingEmployerAccountId == employerAccountId)
                .OrderByDescending(payment => payment.ReceivedTime)
                .Select(payment => payment.ReceivedTime)
                .FirstOrDefaultAsync();
        }

        public async Task<DateTime?> GetLastSentTime(long sendingEmployerAccountId)
        {
            return await _dataContext
                .Payments.Where(payment => payment.SendingEmployerAccountId == sendingEmployerAccountId && payment.EmployerAccountId != sendingEmployerAccountId)
                .OrderByDescending(payment => payment.ReceivedTime)
                .Select(payment => payment.ReceivedTime)
                .FirstOrDefaultAsync();
        }

        public async Task<Dictionary<CalendarPeriod, decimal>> GetPaymentTotals(long employerAccountId)
        {
            var periods = await _dataContext.PaymentAggregation.Where(c => c.EmployerAccountId == employerAccountId)
                .ToDictionaryAsync(k => new CalendarPeriod(k.CollectionPeriod.Year, k.CollectionPeriod.Month),
                    k => k.Amount);
            return periods;
        }
        
        public async Task SaveChanges()
        {
            var stopwatch = new Stopwatch();
            var startTime = DateTime.UtcNow;
            stopwatch.Start();
            await _dataContext.SaveChangesAsync();
            stopwatch.Stop();
            _telemetry.TrackDependency(DependencyType.SqlDatabaseInsert, "Store Payment", startTime, stopwatch.Elapsed, true);
        }

        public async Task CalculatePaymentTotals(long employerAccountId, int collectionPeriodYear, int collectionPeriodMonth)
        {
             var paymentTotal = _dataContext.Payments
                .Where(w =>
                    (w.FundingSource == FundingSource.Levy && w.EmployerAccountId == employerAccountId) ||
                    (w.FundingSource == FundingSource.Transfer && w.SendingEmployerAccountId == employerAccountId))
                .Where(c=>c.CollectionPeriod.Year.Equals(collectionPeriodYear) && c.CollectionPeriod.Month.Equals(collectionPeriodMonth))
                 .Select(c=>c.Amount).Sum();

          
            await _dataContext.Database.ExecuteSqlCommandAsync(@"
                        MERGE PaymentAggregation pa
                            USING
                            (
	                            SELECT
		                            @EmployerAccountId as EmployerAccountId,
		                            @CollectionPeriodYear as CollectionPeriodYear,
		                            @CollectionPeriodMonth as CollectionPeriodMonth,
		                            @Amount as Amount
                            ) s
                            ON pa.EmployerAccountId = s.EmployerAccountId 
	                            AND pa.CollectionPeriodYear = s.CollectionPeriodYear
	                            AND pa.CollectionPeriodMonth = s.CollectionPeriodMonth
                            WHEN NOT MATCHED THEN
	                            INSERT (EmployerAccountId, CollectionPeriodYear, CollectionPeriodMonth, Amount)
	                            VALUES (s.EmployerAccountId, s.CollectionPeriodYear, s.CollectionPeriodMonth, s.Amount)
                            WHEN MATCHED THEN
	                            UPDATE SET Amount = s.Amount;"
                , new SqlParameter("@EmployerAccountId",employerAccountId)
                , new SqlParameter("@CollectionPeriodYear", collectionPeriodYear)
                , new SqlParameter("@CollectionPeriodMonth", collectionPeriodMonth)
                , new SqlParameter("@Amount", paymentTotal));
            
        }
    }
}
