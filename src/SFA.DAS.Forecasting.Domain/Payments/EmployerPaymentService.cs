using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Domain.Payments
{
    public class EmployerPaymentService
    {
		private readonly IEmployerPaymentRepository _paymentStorage;

	    public EmployerPaymentService(IEmployerPaymentRepository paymentStorage)
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

	    public DateTime? GetLastTimeReceivedPayment(List<Payment> payments)
	    {
		    return payments
				.OrderByDescending(payment => payment.ReceivedTime)
				.Select(p => p.ReceivedTime)
				.FirstOrDefault();
	    }
	}
}