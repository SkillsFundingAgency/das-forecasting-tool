using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Forecasting.Data;

namespace SFA.DAS.Forecasting.Application.Infrastructure.RegistrationExtensions;

public static class DatabaseRegistrationExtensions
{
    public static void AddDatabaseRegistration(this IServiceCollection services, string connectionString, string environmentName)
    {
        if (environmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
        {
            services.AddDbContext<ForecastingDataContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Transient);
        }
        else
        {
            services.AddDbContext<ForecastingDataContext>(ServiceLifetime.Transient);
        }


        //services.AddSingleton(new EnvironmentConfiguration(environmentName));

        services.AddTransient<IForecastingDataContext, ForecastingDataContext>(provider => provider.GetService<ForecastingDataContext>());
        services.AddTransient(provider => new Lazy<ForecastingDataContext>(provider.GetService<ForecastingDataContext>()));

    }
}