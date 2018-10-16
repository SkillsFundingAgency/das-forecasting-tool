using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Forecasting.Application.Balance.Services;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Data;
using SFA.DAS.Forecasting.Domain.Balance.Services;
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

            if (ConfigurationHelper.IsDevOrAtEnvironment)
            {
                For<IAccountBalanceService>()
                    .Use<DevAccountBalanceService>();
            }
            else
            {
                For<IAccountBalanceService>()
                    .Use<AccountBalanceService>();
            }
        }
    }
}
