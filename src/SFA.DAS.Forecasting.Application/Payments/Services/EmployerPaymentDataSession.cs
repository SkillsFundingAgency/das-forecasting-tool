using SFA.DAS.Forecasting.Data;
using SFA.DAS.Forecasting.Domain.Payments.Services;
using SFA.DAS.Forecasting.Models.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CalendarPeriod = SFA.DAS.EmployerFinance.Types.Models.CalendarPeriod;

namespace SFA.DAS.Forecasting.Application.Payments.Services
{
    public class EmployerPaymentDataSession : IEmployerPaymentDataSession
    {
        private readonly IForecastingDataContext _dataContext;

        public EmployerPaymentDataSession(IForecastingDataContext dataContext)
        {
            _dataContext = dataContext; 
        }

        public async Task<PaymentModel> Get(long employerAccountId, string paymentId)
        {
            var employerPayment = await _dataContext.Payments
                .FirstOrDefaultAsync(payment => payment.EmployerAccountId == employerAccountId &&
                                                payment.ExternalPaymentId == paymentId);
            return employerPayment;
        }

        public void Store(PaymentModel payment)
        {
            if (payment.Id <= 0)
                _dataContext.Payments.Add(payment);
        }

        public async Task<bool> HasReceivedRecentPayment(long employerAccountId)
        {
            var recentPayment = await _dataContext
                .Payments.Where(payment => (payment.EmployerAccountId == employerAccountId || payment.SendingEmployerAccountId == employerAccountId)
                                                                         && EF.Functions.DateDiffMinute(payment.ReceivedTime, DateTime.UtcNow) <= 5)
                .OrderByDescending(payment => payment.ReceivedTime)
                .FirstOrDefaultAsync();
            
            return recentPayment != null;
        }

        public async Task<bool> HasReceivedRecentPaymentForSendingEmployer(long sendingEmployerAccountId)
        {
            var recentPayment = await _dataContext
                .Payments.Where(payment => (payment.SendingEmployerAccountId == sendingEmployerAccountId && payment.EmployerAccountId != sendingEmployerAccountId)
                                                                                && EF.Functions.DateDiffMinute(payment.ReceivedTime, DateTime.UtcNow) <= 5)
                .OrderByDescending(payment => payment.ReceivedTime)
                .FirstOrDefaultAsync();
                
                return recentPayment != null;
        }

        public async Task<Dictionary<CalendarPeriod, decimal>> GetPaymentTotals(long employerAccountId)
        {
            return GetPaymentTotals(w =>
                (w.FundingSource == FundingSource.Levy && w.EmployerAccountId == employerAccountId) ||
                (w.FundingSource == FundingSource.Transfer && w.SendingEmployerAccountId == employerAccountId));


        }

        private Dictionary<CalendarPeriod, decimal> GetPaymentTotals(Expression<Func<PaymentModel, bool>> wherePredicate)
        {
            return _dataContext.Payments
                .Where(wherePredicate)
                .ToList()
                .GroupBy(g => new { g.CollectionPeriod.Year, g.CollectionPeriod.Month })
                .Select(s => new { s.Key.Year, s.Key.Month, Total = s.Sum(v => v.Amount) }).ToList()
                .ToDictionary(k => new CalendarPeriod(k.Year, k.Month), k => k.Total);
        }
        public Task SaveChanges()
        {
            return Task.FromResult(_dataContext.SaveChanges());
        }
    }
}
