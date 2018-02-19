using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Forecasting.Application.Balance.Services;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.HashingService;
using StructureMap;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Registries
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            if (ConfigurationRegistry.IsDevEnvironment)
            {
                For<IAccountBalanceService>()
                    .Use<DevAccountBalanceService>();
                ForSingletonOf<IHashingService>()
                    .Use<DevHashingService>();
            }
            else
            {
                For<IAccountBalanceService>()
                    .Use<AccountBalanceService>();
                ForSingletonOf<IHashingService>()
                    .Use<HashingService.HashingService>()
                    .Ctor<string>("allowedCharacters").Is(ctx => ctx.GetInstance<IApplicationConfiguration>().AllowedHashstringCharacters)
                    .Ctor<string>("hashstring").Is(ctx => ctx.GetInstance<IApplicationConfiguration>().Hashstring);
            }

            For<IAccountApiClient>()
                .Use<AccountApiClient>()
                .Ctor<AccountApiConfiguration>()
                .Is(ctx => ctx.GetInstance<IApplicationConfiguration>().AccountApi);
        }
    }
}