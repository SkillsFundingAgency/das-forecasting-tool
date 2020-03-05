namespace SFA.DAS.Forecasting.Application.Infrastructure.Configuration
{
	public interface IStartupConfiguration
	{
		string AllowedHashstringCharacters { get; set; }
		string DashboardUrl { get; set; }
		string DatabaseConnectionString { get; set; }
		string Hashstring { get; set; }
		IdentityServerConfiguration Identity { get; set; }
        //string ServiceBusConnectionString { get; set; }
    }
}