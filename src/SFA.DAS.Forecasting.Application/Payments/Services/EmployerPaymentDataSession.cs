using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Data;
using SFA.DAS.Forecasting.Domain.Payments.Services;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Application.Payments.Services
{
    public class EmployerPaymentDataSession : IEmployerPaymentDataSession
    {
        private readonly IForecastingDataContext _dataContext;

        public EmployerPaymentDataSession(IForecastingDataContext dataContext)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        public Task<PaymentModel> Get(long employerAccountId, string paymentId)
        {
            return _dataContext.Payments
                .FirstOrDefaultAsync(payment => payment.EmployerAccountId == employerAccountId && 
                                                payment.ExternalPaymentId == paymentId);
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
            await _dataContext.SaveChangesAsync();
        }
    }
}
