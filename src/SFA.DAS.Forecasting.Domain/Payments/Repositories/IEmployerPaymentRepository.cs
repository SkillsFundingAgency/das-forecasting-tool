using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Payments.Entities;

namespace SFA.DAS.Forecasting.Domain.Payments.Repositories
{
    public interface IEmployerPaymentRepository
    {
	    Task StorePayment(Payment payment);

		Task<List<Payment>> GetPayments(long employerAccountId, int month, int year);
    }
}