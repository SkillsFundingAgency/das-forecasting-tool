using System;
using SFA.DAS.Forecasting.Payments.Application.Apprenticeships;
using SFA.DAS.Forecasting.Payments.Domain.Apprenticeships;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.Forecasting.Payments.Application.Infrastructure.Registries
{
    public class DefaultRegistry: Registry
    {
        public DefaultRegistry()
        {
            //ForSingletonOf<ILog>().Use(new SFA.DAS.NLog.Logger.NLogLogger());
            For<IApprenticeshipRepository>().Use(c =>
                new ApprenticeshipRepository(
                    Environment.GetEnvironmentVariable("DbConnectionString", EnvironmentVariableTarget.Process)));
            For<IPaymentEventMapper>().Use<PaymentEventMapper>();
            For<IApprenticeshipService>().Use<ApprenticeshipService>();
        }
    }
}