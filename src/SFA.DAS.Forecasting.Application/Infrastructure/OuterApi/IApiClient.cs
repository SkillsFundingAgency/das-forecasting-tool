using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;

public interface IApiClient
{
    Task<TResponse> Get<TResponse>(IGetApiRequest request);
}