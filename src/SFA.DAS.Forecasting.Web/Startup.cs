using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.EmployerUrlHelper.DependencyResolution;
using SFA.DAS.Forecasting.Application.Infrastructure.RegistrationExtensions;
using SFA.DAS.Forecasting.Core.Configuration;
using SFA.DAS.Forecasting.Web.AppStart;
using SFA.DAS.Forecasting.Web.Configuration;
using SFA.DAS.Forecasting.Web.Extensions;
using SFA.DAS.Forecasting.Web.Filters;
using SFA.DAS.GovUK.Auth.AppStart;

namespace SFA.DAS.Forecasting.Web
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfigurationRoot _configuration;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _environment = environment;
            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .SetBasePath(Directory.GetCurrentDirectory());
#if DEBUG
            if (!configuration.IsDev())
            {
                config.AddJsonFile("appsettings.json", false)
                    .AddJsonFile("appsettings.Development.json", true);
            }
#endif

            config.AddEnvironmentVariables();
            if (!configuration.IsDev())
            {
                config.AddAzureTableStorage(options =>
                    {
                        options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                        options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                        options.EnvironmentName = configuration["EnvironmentName"];
                        options.PreFixConfigurationKeys = false;
                        options.ConfigurationKeysRawJsonResult = new[] { "SFA.DAS.Encoding" };
                    }
                );
            }

            _configuration = config.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var forecastingConfiguration = _configuration
                .GetSection(nameof(ForecastingConfiguration))
                .Get<ForecastingConfiguration>();
            var forecastingConnectionStrings = _configuration
                .GetSection(nameof(ForecastingConnectionStrings))
                .Get<ForecastingConnectionStrings>();
            services.AddConfigurationOptions(_configuration);
            services.AddFluentValidation();
            services.AddOrchestrators();

            services.AddDatabaseRegistration(forecastingConnectionStrings.DatabaseConnectionString, _configuration["EnvironmentName"]);

            services.AddApplicationServices(forecastingConfiguration);

            services.AddCosmosDbServices(forecastingConnectionStrings.CosmosDbConnectionString);

            services.AddDomainServices();

            services.AddEmployerUrlHelper();

            services.AddAuthenticationServices();
        
            services.AddAndConfigureGovUkAuthentication(_configuration,
                typeof(EmployerAccountPostAuthenticationClaimsHandler),
                "","/accounts/SignIn-Stub");
            services.AddMaMenuConfiguration(RouteNames.SignOut, _configuration["ResourceEnvironmentName"]);
        

            services.AddLogging();
            services.Configure<IISServerOptions>(options => { options.AutomaticAuthentication = false; });



            services.Configure<RouteOptions>(options =>
                {

                }).AddMvc(options =>
                {
                    options.Filters.Add(new GoogleAnalyticsFilter());
                    if (!_configuration.IsDev())
                    {
                        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                    }

                });

            services.AddApplicationInsightsTelemetry();

            if (!_environment.IsDevelopment())
            {
                services.AddHealthChecks();
                services.AddDataProtection(_configuration);
            }

#if DEBUG
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
#endif

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHealthChecks();
                app.UseExceptionHandler("/Error/500");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.Use(async (context, next) =>
            {
                const string notFoundPath = "/error/404";
                if ((string)context.Request.Path != notFoundPath)
                {
                    context.Response.Redirect(notFoundPath);
                    return;
                }
                
                await next();
                
            });

            app.UseRouting();

            app.UseAuthorization();
    
            app.UseEndpoints(builder => { builder.MapDefaultControllerRoute(); });
        }

        public class Constants
        {
            public static string DefaultEstimationName = "default";
        }
    }
}
