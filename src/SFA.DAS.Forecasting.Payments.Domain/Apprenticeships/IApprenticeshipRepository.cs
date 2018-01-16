using System.Threading.Tasks;
using SFA.DAS.Forecasting.Payments.Domain.Entities;

namespace SFA.DAS.Forecasting.Payments.Domain.Apprenticeships
{
    public interface IApprenticeshipRepository
    {
        Task InsertOrUpdatePayment(PaymentApprenticeship record);
    }
}