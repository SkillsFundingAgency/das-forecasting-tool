using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Domain.Payments.Services
{
    public interface IEmployerPaymentDataService
    {
        Task StoreEmployerPayment(Payment employerPayment);
	    Task<List<Payment>> GetEmployerPayments(long employerAccountId, int month, int year);
    }
}