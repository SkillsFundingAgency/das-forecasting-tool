﻿using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Data;
using SFA.DAS.Forecasting.Domain.Payments.Services;
using SFA.DAS.Forecasting.Models.Payments;

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
                .Payments.Where(payment => payment.EmployerAccountId == employerAccountId)
                .OrderByDescending(payment => payment.ReceivedTime)
                .Select(payment => payment.ReceivedTime)
                .FirstOrDefaultAsync();
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
    }
}
