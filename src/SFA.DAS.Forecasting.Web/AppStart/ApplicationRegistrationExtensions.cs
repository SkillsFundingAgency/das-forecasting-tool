using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerFinance.Types.Models;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Application.Commitments.Services;
using SFA.DAS.Forecasting.Application.ExpiredFunds.Service;
using SFA.DAS.Forecasting.Core.Configuration;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Commitments.Services;
using SFA.DAS.HashingService;

namespace SFA.DAS.Forecasting.Web;

public static class ApplicationRegistrationExtensions
{
    public static void AddApplicationServices(this IServiceCollection services, ForecastingConfiguration configuration)
    {
        services.AddTransient<IApprenticeshipCourseDataService, ApprenticeshipCourseDataService>();
        
        services.AddTransient<IAccountApiClient, AccountApiClient>();//TODO FAI-625

        services.AddTransient<IEmployerCommitmentsRepository, EmployerCommitmentsRepository>();
        services.AddTransient<ICommitmentsDataService, CommitmentsDataService>();
        services.AddTransient<IExpiredFundsService, ExpiredFundsService>();
        services.AddTransient<IExpiredFunds, ExpiredFunds>();
        
        services.AddTransient<IHashingService>(_ => new HashingService.HashingService(configuration.AllowedCharacters, configuration.HashString));
    }
}