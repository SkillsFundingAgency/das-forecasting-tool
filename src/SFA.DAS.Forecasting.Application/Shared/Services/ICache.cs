using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Application.Shared.Services
{
    public interface ICache
    {
        Task<bool> Exists(string key);
        Task<T> Get<T>(string key);
        Task Set<T>(string key, T customType, int secondsInCache = 600);
    }
}
