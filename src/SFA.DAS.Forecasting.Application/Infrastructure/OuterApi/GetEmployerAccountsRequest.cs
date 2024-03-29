using System.Web;

namespace SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;

public class GetEmployerAccountsRequest : IGetApiRequest
{
    private readonly string _email;
    private readonly string _userId;

    public GetEmployerAccountsRequest(string email, string userId)
    {
        _email = HttpUtility.UrlEncode(email);
        _userId = userId;
    }

    public string GetUrl => $"accountusers/{_userId}/accounts?email={_email}";
}