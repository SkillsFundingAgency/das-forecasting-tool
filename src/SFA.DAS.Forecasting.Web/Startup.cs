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
using SFA.DAS.Forecasting.Core.Configuration;
using SFA.DAS.Forecasting.Web.Extensions;
using SFA.DAS.Forecasting.Web.Filters;
using SFA.DAS.Forecasting.Web.Orchestrators.Mappers;
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
                        options.EnvironmentName = configuration["Environment"];
                        options.PreFixConfigurationKeys = false;
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
            var identityServerConfiguration = _configuration
                .GetSection(nameof(IdentityServerConfiguration))
                .Get<IdentityServerConfiguration>();
            services.AddConfigurationOptions(_configuration);
            services.AddFluentValidation();
            services.AddOrchestrators();

            services.AddTransient<IForecastingMapper, ForecastingMapper>();

            services.AddDatabaseRegistration(forecastingConfiguration, _configuration["Environment"]);
            
            services.AddApplicationServices(forecastingConfiguration);
            
            services.AddCosmosDbServices(forecastingConfiguration);
            
            services.AddDomainServices();

            services.AddAuthenticationServices();

            if (_configuration["ForecastingConfiguration:UseGovSignIn"] != null &&
                _configuration["ForecastingConfiguration:UseGovSignIn"]
                    .Equals("true", StringComparison.CurrentCultureIgnoreCase))
            {
                services.AddAndConfigureGovUkAuthentication(_configuration,
                    $"{typeof(Startup).Assembly.GetName().Name}.Auth",
                    typeof(EmployerAccountPostAuthenticationClaimsHandler));
            }
            else
            {
                services.AddAndConfigureEmployerAuthentication(identityServerConfiguration);    
            }

            services.AddLogging();
            services.Configure<IISServerOptions>(options => { options.AutomaticAuthentication = false; });
            
            services.AddMaMenuConfiguration("signout", identityServerConfiguration.ClientId,_configuration["Environment"]);
            
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
                if (context.Response.Headers.ContainsKey("X-Frame-Options"))
                {
                    context.Response.Headers.Remove("X-Frame-Options");
                }

                context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");

                await next();

                if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
                {
                    //Re-execute the request so the user gets the error page
                    var originalPath = context.Request.Path.Value;
                    context.Items["originalPath"] = originalPath;
                    context.Request.Path = "/error/404";
                    await next();
                }
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
