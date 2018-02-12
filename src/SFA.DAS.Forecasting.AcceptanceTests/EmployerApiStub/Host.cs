using Nancy.Hosting.Self;
using System;

namespace SFA.DAS.Forecasting.AcceptanceTests.EmployerApiStub
{
    public class ApiHost : IDisposable
    {
        NancyHost _host;
        public ApiHost()
        {
            var config = new HostConfiguration { RewriteLocalhost = false };
            config.UrlReservations.CreateAutomatically = true;
            _host = new NancyHost(config, new Uri("http://localhost:50002"));
            _host.Start();
        }

        public void Dispose()
        {
            _host.Stop();
            _host = null;
        }
    }
}
