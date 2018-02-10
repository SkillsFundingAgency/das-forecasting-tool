using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Domain.Balance.Services
{
    public interface IBalanceDataService
    {
        Task<Model.Balance> Get(long employerAccountId);
        Task Store(Model.Balance balance);
    }
}