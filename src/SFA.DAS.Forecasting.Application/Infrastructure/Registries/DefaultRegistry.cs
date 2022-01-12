using System.Net.Http;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerFinance.Types.Models;
using SFA.DAS.Forecasting.Application.Balance.Services;
using SFA.DAS.Forecasting.Application.ExpiredFunds.Service;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;
using SFA.DAS.Forecasting.Application.Payments.Services;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Data;
using SFA.DAS.Forecasting.Domain.Balance.Services;
using SFA.DAS.HashingService;
using SFA.DAS.Http;
using SFA.DAS.Provider.Events.Api.Client;
using StructureMap;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Registries
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            var azureTokenProvider = new Microsoft.Azure.Services.AppAuthentication.AzureServiceTokenProvider();
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

            var apiConfig = ConfigurationHelper.GetAccountApiConfiguration();
            For<IAccountApiClient>()
                .Use<AccountApiClient>()
                .Ctor<IAccountApiConfiguration>()
                .Is(apiConfig);


            For<IEmployerDatabaseService>()
                .Use<EmployerDatabaseService>();

            System.Data.Entity.DbConfiguration.Loaded += (_, a) =>
            {
                a.ReplaceService<System.Data.Entity.Infrastructure.IDbConnectionFactory>((s, k) => new CustomDbConnectionFactory(ConfigurationHelper.IsDevEnvironment));
            };

            For<IForecastingDataContext>()
                .Use<ForecastingDataContext>()
                .Ctor<IApplicationConnectionStrings>("config")
                .Is(ctx => ctx.GetInstance<IApplicationConnectionStrings>())
                .ContainerScoped();

            For<IExpiredFundsService>()
                .Use<ExpiredFundsService>();

            For<IExpiredFunds>()
                .Use<EmployerFinance.Types.Models.ExpiredFunds>();

            For<IApiClient>()
                .Use<ApiClient>()
                .Ctor<HttpClient>("httpClient")
                .Is(new HttpClient());
        }
    }
}