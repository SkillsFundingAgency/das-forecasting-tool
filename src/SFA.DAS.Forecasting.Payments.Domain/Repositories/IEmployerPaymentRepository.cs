using System.Threading.Tasks;
using SFA.DAS.Forecasting.Payments.Domain.Entities;

namespace SFA.DAS.Forecasting.Payments.Domain.Repositories
{
    public interface IEmployerPaymentRepository
    {
	    Task StorePayment(Payment payment);
    }
}