using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.Encoding;
using SFA.DAS.Forecasting.Core.Configuration;

namespace SFA.DAS.Forecasting.Web;

public static class AddConfigurationExtensions
{
    public static void AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.Configure<ForecastingConfiguration>(configuration.GetSection(nameof(ForecastingConfiguration)));
        services.AddSingleton(cfg => cfg.GetService<IOptions<ForecastingConfiguration>>().Value);

        services.Configure<ForecastingConnectionStrings>(configuration.GetSection(nameof(ForecastingConnectionStrings)));
        services.AddSingleton(cfg => cfg.GetService<IOptions<ForecastingConnectionStrings>>().Value);
        
        services.Configure<IdentityServerConfiguration>(configuration.GetSection(nameof(IdentityServerConfiguration)));
        services.Configure<OuterApiConfiguration>(configuration.GetSection(nameof(OuterApiConfiguration)));
        services.AddSingleton(cfg => cfg.GetService<IOptions<OuterApiConfiguration>>().Value);
        
        var encodingConfigJson = configuration.GetSection("SFA.DAS.Encoding").Value;
        var encodingConfig = JsonConvert.DeserializeObject<EncodingConfig>(encodingConfigJson);
        services.AddSingleton(encodingConfig);
    }
}