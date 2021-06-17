using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Forecasting.Application.Balance.Services;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Data;
using SFA.DAS.Forecasting.Domain.Balance.Services;
using SFA.DAS.HashingService;
using StructureMap;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Registries
{
    public class DefaultWebRegistry : Registry
    {
        public DefaultWebRegistry()
        {

            For<IForecastingDataContext>()
                .Use<ForecastingDataContext>()
                .Ctor<IApplicationConnectionStrings>("config")
                .Is(ctx => ctx.GetInstance<IApplicationConnectionStrings>())
                .ContainerScoped();

            var apiConfig = ConfigurationHelper.GetAccountApiConfiguration();
            For<IAccountApiClient>()
                .Use<AccountApiClient>()
                .Ctor<IAccountApiConfiguration>()
                .Is(apiConfig);

            if (ConfigurationHelper.IsDevEnvironment)
            {
                For<IAccountBalanceService>()
                    .Use<DevAccountBalanceService>();
                For<IHashingService>()
                    .Use<DevHashingService>();
            }
            else
            {
                For<IAccountBalanceService>()
                    .Use<AccountBalanceService>();
                For<IHashingService>()
                    .Use<HashingService.HashingService>()
                    .Ctor<string>("allowedCharacters").Is(ctx => ctx.GetInstance<IApplicationConfiguration>().AllowedHashStringCharacters)
                    .Ctor<string>("hashstring").Is(ctx => ctx.GetInstance<IApplicationConfiguration>().HashString);
            }
        }
    }
}
