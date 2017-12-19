using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SFA.DAS.Forecasting.Web.Startup))]
namespace SFA.DAS.Forecasting.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
