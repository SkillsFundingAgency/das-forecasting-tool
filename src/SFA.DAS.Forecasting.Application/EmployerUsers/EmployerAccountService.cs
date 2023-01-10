using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.EmployerUsers.ApiResponse;
using SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;

namespace SFA.DAS.Forecasting.Application.EmployerUsers;

public interface IEmployerAccountService
{
    Task<EmployerUserAccounts> GetUserAccounts(string userId, string email);
}

public class EmployerAccountService : IEmployerAccountService
{
    private readonly IApiClient _apiClient;

    public EmployerAccountService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }
    public async Task<EmployerUserAccounts> GetUserAccounts(string userId, string email)
    {
        var actual = await _apiClient.Get<GetUserAccountsResponse>(new GetEmployerAccountsRequest(email, userId));

        return actual;
    }
}