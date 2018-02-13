using System.Threading.Tasks;
using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Domain.Payments.Services
{
    public interface ICommitmentDataService
    {
        Task StoreCommitment(Commitment commitment);
    }
}