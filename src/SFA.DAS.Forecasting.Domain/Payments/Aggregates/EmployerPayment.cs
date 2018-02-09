using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Payments.Entities;
using SFA.DAS.Forecasting.Domain.Payments.Repositories;

namespace SFA.DAS.Forecasting.Domain.Payments.Aggregates
{
    public class EmployerPayment
    {
		private readonly IEmployerPaymentRepository _paymentStorage;

	    public EmployerPayment(IEmployerPaymentRepository paymentStorage)
	    {
		    _paymentStorage = paymentStorage;
	    }

	    public async Task AddPayment(Payment payment)
	    {
		    await _paymentStorage.StorePayment(payment);
	    }

		public async Task<List<Payment>> GetPaymentsForEmployerPeriod(long employerAccountId, int month, int year)
	    {
		    return await _paymentStorage.GetPayments(employerAccountId, month, year);
	    }
	}
}