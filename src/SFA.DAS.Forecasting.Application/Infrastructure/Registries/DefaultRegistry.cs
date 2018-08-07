using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Forecasting.Application.Balance.Services;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Payments.Services;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Data;
using SFA.DAS.Forecasting.Domain.Balance.Services;
using SFA.DAS.HashingService;
using SFA.DAS.Provider.Events.Api.Client;
using StructureMap;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Registries
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            if (ConfigurationHelper.IsDevOrAtEnvironment)
            {
                For<IPaymentsEventsApiClient>().Use<DevPaymentsEventsApiClient>()
                    .Ctor<IPaymentsEventsApiConfiguration>().Is(ctx => ctx.GetInstance<IApplicationConfiguration>().PaymentEventsApi);
                For<IAccountBalanceService>()
                    .Use<DevAccountBalanceService>();
                For<IHashingService>()
                    .Use<DevHashingService>();
            }
            else
            {
                For<IPaymentsEventsApiClient>().Use<PaymentsEventsApiClient>()
                    .Ctor<IPaymentsEventsApiConfiguration>().Is(ctx => ctx.GetInstance<IApplicationConfiguration>().PaymentEventsApi);
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

            For<IPaymentsEventsApiClient>().Use<PaymentsEventsApiClient>()
                .Ctor<IPaymentsEventsApiConfiguration>().Is(ctx => ctx.GetInstance<IApplicationConfiguration>().PaymentEventsApi);


            For<IStandardApiClient>()
                .Use<StandardApiClient>()
                .Ctor<string>("baseUri")
                .Is(ctx => ctx.GetInstance<IApplicationConfiguration>().ApprenticeshipsApiBaseUri);

            For<IForecastingDataContext>()
                .Use<ForecastingDataContext>()
                .Ctor<IApplicationConnectionStrings>("config")
                .Is(ctx => ctx.GetInstance<IApplicationConnectionStrings>())
                .ContainerScoped();

            For<IEmployerDatabaseService>()
                .Use<EmployerDatabaseService>();
        }
    }
}