//using SFA.DAS.Commitments.Api.Client;
//using SFA.DAS.Commitments.Api.Client.Configuration;
//using SFA.DAS.Commitments.Api.Client.Interfaces;
//using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
//using SFA.DAS.Http;
//using SFA.DAS.Http.TokenGenerators;
//using StructureMap;
//using System.Net.Http;

//namespace SFA.DAS.Forecasting.Application.Infrastructure.Registries
//{
//    public class CommitmentsRegistry : Registry
//    {
//        public CommitmentsRegistry()
//        {
//            For<IEmployerCommitmentApi>().Use<EmployerCommitmentApi>()
//                .Ctor<ICommitmentsApiClientConfiguration>("configuration")
//                .Is(ctx => ctx.GetInstance<IApplicationConfiguration>().CommitmentsApi)
//                .Ctor<HttpClient>("client")
//                .Is(ctx => GetHttpClient(ctx));
//        }

//        private HttpClient GetHttpClient(IContext context)
//        {   
//            var config = ConfigurationHelper.GetCommitmentsApiConfiguration();
//            var bearerToken = (IGenerateBearerToken)new JwtBearerTokenGenerator(config);

//            var httpClientBuilder = string.IsNullOrWhiteSpace(config.ClientId)
//               ? new HttpClientBuilder().WithBearerAuthorisationHeader(new JwtBearerTokenGenerator(config))
//               : new HttpClientBuilder().WithBearerAuthorisationHeader(new AzureActiveDirectoryBearerTokenGenerator(config));

//            return httpClientBuilder
//                .WithDefaultHeaders()
//                .Build();
//        }
//    }
//}
