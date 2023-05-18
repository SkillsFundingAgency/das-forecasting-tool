using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Forecasting.Web.Orchestrators;
using SFA.DAS.Forecasting.Web.Orchestrators.Estimations;

namespace SFA.DAS.Forecasting.Web;

public static class RegisterOrchestratorsExtensions
{
    public static void AddOrchestrators(this IServiceCollection services)
    {
        services.AddTransient<IAddApprenticeshipOrchestrator, AddApprenticeshipOrchestrator>();
        services.AddTransient<IEstimationOrchestrator, EstimationOrchestrator>();
        services.AddTransient<IForecastingOrchestrator, ForecastingOrchestrator>();
    }
}