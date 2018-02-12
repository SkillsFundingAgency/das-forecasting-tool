using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Domain.Balance.Services
{
    public interface IBalanceDataService
    {
        Task<Models.Balance.Balance> Get(long employerAccountId);
        Task Store(Models.Balance.Balance balance);
    }
}