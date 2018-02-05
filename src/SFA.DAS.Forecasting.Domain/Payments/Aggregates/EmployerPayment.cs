using System;
using System.Collections.Generic;
using System.Linq;
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

		public List<Payment> GetPaymentsForEmployerPeriod(string employerAccountId, int month, int year)
	    {
		    return _paymentStorage.GetPayments(employerAccountId, month, year);
	    }
    }
}