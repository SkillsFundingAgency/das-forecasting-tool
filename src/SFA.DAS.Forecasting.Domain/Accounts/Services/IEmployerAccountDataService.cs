using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Accounts.Model;

namespace SFA.DAS.Forecasting.Domain.Accounts.Services
{
    public interface IEmployerAccountDataService
    {
        Task<EmployerAccount> Get(long employerAccountId);
        Task Store(EmployerAccount employerAccount);
    }
}