using System;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Application.Shared.Services
{
    public class InMemoryCache : ICache
    {
        public Task<bool> Exists(string key)
        {
            var value = MemoryCache.Default.Get(key);

            return Task.FromResult(value != null);
        }

        public Task<T> Get<T>(string key)
        {
            return Task.FromResult((T)MemoryCache.Default.Get(key));
        }

        public Task Set<T>(string key, T customType, int secondsInCache = 600)
        {
            MemoryCache.Default.Set(key, customType, new CacheItemPolicy { AbsoluteExpiration = new DateTimeOffset(DateTime.UtcNow.AddSeconds(secondsInCache)) });

            return Task.FromResult<object>(null);
        }
    }
}
