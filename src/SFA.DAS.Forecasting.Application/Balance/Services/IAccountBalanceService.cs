using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Application.Balance.Services
{
    public interface IAccountBalanceService
    {
        Task<decimal> GetAccountBalance(long accountId);
    }
}