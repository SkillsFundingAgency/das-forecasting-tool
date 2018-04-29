using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Payments.Services;

namespace SFA.DAS.Forecasting.Domain.Payments
{
    public interface IEmployerPaymentsRepository
    {
        EmployerPayments Get(long employerAccountId);
        Task Store(EmployerPayment payment);
    }

    public class EmployerPaymentsRepository : IEmployerPaymentsRepository
    {
        private readonly IEmployerPaymentDataSession _dataSession;

        public EmployerPaymentsRepository(IEmployerPaymentDataSession dataSession)
        {
            _dataSession = dataSession ?? throw new ArgumentNullException(nameof(dataSession));
        }

        public EmployerPayments Get(long employerAccountId)
        {
            return new EmployerPayments(employerAccountId, _dataSession);
        }

        public async Task Store(EmployerPayment payment)
        {
            _dataSession.Store(payment.Model);
            await _dataSession.SaveChanges();
        }
    }
}