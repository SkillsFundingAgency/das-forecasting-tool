using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Payments.Entities;

namespace SFA.DAS.Forecasting.Domain.Payments.Repositories
{
    public interface IEmployerPaymentRepository
    {
	    Task StorePayment(Payment payment);

	    List<Payment> GetPayments(string employerId);

		List<Payment> GetPayments(string employerAccountId, int month, int year);
    }
}