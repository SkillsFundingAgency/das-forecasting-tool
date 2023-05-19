using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Forecasting.Core.Configuration;
using StackExchange.Redis;

namespace SFA.DAS.Forecasting.Web;

public static class AddDataProtectionExtensions
{
    public static void AddDataProtection(this IServiceCollection services, IConfiguration configuration)
    {
            
        var config = configuration.GetSection(nameof(ForecastingConfiguration))
            .Get<ForecastingConfiguration>();

        if (config != null 
            && !string.IsNullOrEmpty(config.DataProtectionKeysDatabase) 
            && !string.IsNullOrEmpty(config.RedisConnectionString))
        {
            var redisConnectionString = config.RedisConnectionString;
            var dataProtectionKeysDatabase = config.DataProtectionKeysDatabase;

            var redis = ConnectionMultiplexer
                .Connect($"{redisConnectionString},{dataProtectionKeysDatabase}");

            services.AddDataProtection()
                .SetApplicationName("das-employer")
                .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
        }
    }
}