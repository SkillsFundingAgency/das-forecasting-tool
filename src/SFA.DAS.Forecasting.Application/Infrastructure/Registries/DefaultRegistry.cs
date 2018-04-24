using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Forecasting.Application.Balance.Services;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Shared.Services;
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
                    .Ctor<string>("allowedCharacters").Is(ctx => ctx.GetInstance<IApplicationConfiguration>().AllowedHashstringCharacters)
                    .Ctor<string>("hashstring").Is(ctx => ctx.GetInstance<IApplicationConfiguration>().Hashstring);
            }

            var apiConfig = ConfigurationHelper.GetAccountApiConfiguration();
            For<IAccountApiClient>()
                .Use<AccountApiClient>()
                .Ctor<IAccountApiConfiguration>()
                .Is(apiConfig);

            For<IPaymentsEventsApiClient>().Use<PaymentsEventsApiClient>()
                .Ctor<IPaymentsEventsApiConfiguration>().Is(ctx => ctx.GetInstance<IApplicationConfiguration>().PaymentEventsApi);

            For<IEmployerDatabaseService>()
                .Use<EmployerDatabaseService>();

            For<IStandardApiClient>()
                .Use<StandardApiClient>()
                .Ctor<string>("baseUri")
                .Is(ctx => ctx.GetInstance<IApplicationConfiguration>().ApprenticeshipsApiBaseUri);
        }
    }
}