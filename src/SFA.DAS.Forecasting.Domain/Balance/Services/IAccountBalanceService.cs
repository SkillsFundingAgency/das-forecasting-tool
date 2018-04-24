using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Domain.Balance.Services
{
    public interface IAccountBalanceService
    {
        Task<Models.Balance.BalanceModel> GetAccountBalance(long accountId);
    }
}