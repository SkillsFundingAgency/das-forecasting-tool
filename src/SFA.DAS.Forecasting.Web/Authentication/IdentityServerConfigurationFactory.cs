using SFA.DAS.EmployerUsers.WebClientComponents;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;

namespace SFA.DAS.Forecasting.Web.Authentication
{
	public class IdentityServerConfigurationFactory : ConfigurationFactory
	{
		private readonly StartupConfiguration _configuration;

		public IdentityServerConfigurationFactory(StartupConfiguration configuration)
		{
			_configuration = configuration;
		}

		public override ConfigurationContext Get()
		{
			return new ConfigurationContext { AccountActivationUrl = _configuration.Identity.BaseAddress.Replace("/identity", "") + _configuration.Identity.AccountActivationUrl };
		}
	}
}