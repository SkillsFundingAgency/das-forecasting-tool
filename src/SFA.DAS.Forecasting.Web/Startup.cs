using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Forecasting.Core.Configuration;
using SFA.DAS.Forecasting.Web.Extensions;

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
            services.AddFluentValidation();
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
            public IdentityServerConfiguration Configuration { get; set; }

            private readonly string _baseUrl;

            public Constants(IdentityServerConfiguration configuration)
            {
                _baseUrl = configuration.ClaimIdentifierConfiguration.ClaimsBaseUrl;
                Configuration = configuration;
            }

            public static string UserExternalIdClaimKeyName = "sub";
            public static string AccountHashedIdRouteKeyName = "HashedAccountId";
            public static string DefaultEstimationName = "default";

            public string AuthorizeEndpoint() => $"{Configuration.BaseAddress}{Configuration.AuthorizeEndPoint}";

            public string ChangeEmailLink() => Configuration.BaseAddress.Replace("/identity", "") +
                                               string.Format(Configuration.ChangeEmailLink, Configuration.ClientId);

            public string ChangePasswordLink() => Configuration.BaseAddress.Replace("/identity", "") +
                                                  string.Format(Configuration.ChangePasswordLink,
                                                      Configuration.ClientId);

            public string DisplayName() => _baseUrl + Configuration.ClaimIdentifierConfiguration.DisplayName;
            public string Email() => _baseUrl + Configuration.ClaimIdentifierConfiguration.Email;
            public string FamilyName() => _baseUrl + Configuration.ClaimIdentifierConfiguration.FamilyName;
            public string GivenName() => _baseUrl + Configuration.ClaimIdentifierConfiguration.GivenName;
            public string Id() => _baseUrl + Configuration.ClaimIdentifierConfiguration.Id;
            public string LogoutEndpoint() => $"{Configuration.BaseAddress}{Configuration.LogoutEndpoint}";

            public string RegisterLink() => Configuration.BaseAddress.Replace("/identity", "") +
                                            string.Format(Configuration.RegisterLink, Configuration.ClientId);

            public string RequiresVerification() => _baseUrl + "requires_verification";
            public string TokenEndpoint() => $"{Configuration.BaseAddress}{Configuration.TokenEndpoint}";
            public string UserInfoEndpoint() => $"{Configuration.BaseAddress}{Configuration.UserInfoEndpoint}";


        }
    }
}
