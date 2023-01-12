using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Forecasting.Application.Balance.Services;
using SFA.DAS.Forecasting.Application.Estimations.Services;
using SFA.DAS.Forecasting.Application.Levy.Services;
using SFA.DAS.Forecasting.Application.Payments.Services;
using SFA.DAS.Forecasting.Application.Projections.Services;
using SFA.DAS.Forecasting.Core.Configuration;
using SFA.DAS.Forecasting.Domain.Balance;
using SFA.DAS.Forecasting.Domain.Balance.Services;
using SFA.DAS.Forecasting.Domain.Estimations;
using SFA.DAS.Forecasting.Domain.Estimations.Services;
using SFA.DAS.Forecasting.Domain.Estimations.Validation.VirtualApprenticeships;
using SFA.DAS.Forecasting.Domain.Levy.Services;
using SFA.DAS.Forecasting.Domain.Payments.Services;
using SFA.DAS.Forecasting.Domain.Projections.Services;
using SFA.DAS.Forecasting.Domain.Shared;

namespace SFA.DAS.Forecasting.Web;

public static class DomainRegistrationExtensions
{
    public static void AddDomainServices(this IServiceCollection services)
    {
        services.AddTransient<IAccountProjectionDataSession, AccountProjectionDataSession>();
        services.AddTransient<ICurrentBalanceRepository, CurrentBalanceRepository>();
        services.AddTransient<IBalanceDataService, BalanceDataService>();
        services.AddTransient<IAccountBalanceService, AccountBalanceService>();
        services.AddTransient<ILevyDataSession, LevyDataSession>();
        services.AddTransient<IEmployerPaymentDataSession, EmployerPaymentDataSession>();
        services.AddTransient<ICommitmentModelListBuilder, CommitmentModelListBuilder>();
        services.AddTransient<IAccountEstimationRepository, AccountEstimationRepository>();
        services.AddTransient<IAccountEstimationDataService, AccountEstimationDataService>();
        services.AddTransient<IVirtualApprenticeshipValidator, VirtualApprenticeshipValidator>();
        services.AddTransient<IDateTimeService, DateTimeService>();
        services.AddTransient<IAccountEstimationProjectionRepository, AccountEstimationProjectionRepository>();
    }
}