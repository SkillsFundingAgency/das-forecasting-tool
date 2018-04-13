using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.Forecasting.Application.Balance.Services
{
    public interface IAccountBalanceService
    {
        Task<AccountDetailViewModel> GetAccountBalance(long accountId);
    }
}