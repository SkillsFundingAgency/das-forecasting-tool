using System;
using System.Configuration;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.HashingService;
using StructureMap;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Registries
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            ForSingletonOf<IHashingService>()
                .Use<HashingService.HashingService>()
                .Ctor<string>("allowedCharacters").Is(ctx => ctx.GetInstance<IApplicationConfiguration>().AllowedHashstringCharacters)
                .Ctor<string>("hashstring").Is(ctx => ctx.GetInstance<IApplicationConfiguration>().Hashstring);
            For<IAccountApiClient>()
                .Use<AccountApiClient>()
                .Ctor<AccountApiConfiguration>()
                .Is(ctx => ctx.GetInstance<IApplicationConfiguration>().AccountApi);
        }
    }
}