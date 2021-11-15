using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Client.DependencyResolution;
using SFA.DAS.Forecasting.Application.LocalDevRegistry;
using StructureMap;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Registries
{
    public class CommitmentsRegistry : Registry
    {
        public CommitmentsRegistry()
        {
            if (ConfigurationHelper.ByPassMI)
            {
                For<ICommitmentsApiClient>().Use(c => c.GetInstance<ICommitmentsApiClientFactory>().CreateClient()).Singleton();
                For<ICommitmentsApiClientFactory>().Use<LocalDevCommitmentApiClientFactory>();
            }
            else
            {
                IncludeRegistry<CommitmentsApiClientRegistry>();
            }
        }
    }
}
