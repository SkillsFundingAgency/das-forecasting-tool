using System;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Forecasting.Data;

namespace SFA.DAS.Forecasting.Application.Infrastructure.RegistrationExtensions;

public static class DatabaseRegistrationExtensions
{
    public static void AddDatabaseRegistration(this IServiceCollection services, string connectionString, string environmentName)
    {
        var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
        bool useManagedIdentity = !connectionStringBuilder.IntegratedSecurity && string.IsNullOrEmpty(connectionStringBuilder.UserID);

        if (!useManagedIdentity)
        {
            services.AddDbContext<ForecastingDataContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Transient);
        }
        else
        {
            services.AddDbContext<ForecastingDataContext>(ServiceLifetime.Transient);

            services.AddSingleton(new ChainedTokenCredential(
                new ManagedIdentityCredential(),
                new AzureCliCredential(),
                new VisualStudioCodeCredential(),
                new VisualStudioCredential())
            );
        }

        services.AddTransient<IForecastingDataContext, ForecastingDataContext>(provider => provider.GetService<ForecastingDataContext>());
        services.AddTransient(provider => new Lazy<ForecastingDataContext>(provider.GetService<ForecastingDataContext>()));



    }
}