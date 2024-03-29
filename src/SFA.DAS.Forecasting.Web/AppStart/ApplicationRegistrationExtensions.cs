using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerFinance.Types.Models;
using SFA.DAS.Encoding;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Application.Commitments.Services;
using SFA.DAS.Forecasting.Application.ExpiredFunds.Service;
using SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;
using SFA.DAS.Forecasting.Core.Configuration;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Commitments.Services;
using SFA.DAS.Forecasting.Web.Orchestrators.Mappers;

namespace SFA.DAS.Forecasting.Web;

public static class ApplicationRegistrationExtensions
{
    public static void AddApplicationServices(this IServiceCollection services, ForecastingConfiguration configuration)
    {
        services.AddTransient<IApprenticeshipCourseDataService, ApprenticeshipCourseDataService>();
        
        services.AddTransient<IEmployerCommitmentsRepository, EmployerCommitmentsRepository>();
        services.AddTransient<ICommitmentsDataService, CommitmentsDataService>();
        services.AddTransient<IExpiredFundsService, ExpiredFundsService>();
        services.AddTransient<IExpiredFunds, ExpiredFunds>();
        
        services.AddTransient<IForecastingMapper, ForecastingMapper>();
        services.AddTransient<IEncodingService, EncodingService>();

        services.AddHttpClient<IApiClient, ApiClient>();
    }
}