using NServiceBus;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Messages.Projections;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Application.Projections.Handlers
{
    public class PublishAccountProjectionHandler
    {

        private readonly IApplicationConfiguration _config;
        private readonly ITelemetry _telemetry;
        private readonly IMessageSession _messageSession;



        public PublishAccountProjectionHandler(IApplicationConfiguration config, ITelemetry telemetry, IMessageSession messageSession)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
            _messageSession = messageSession ?? throw new ArgumentNullException(nameof(messageSession));
        }

        public async Task Handle(AccountProjectionCreatedEvent message)
        {
            _telemetry.AddEmployerAccountId(message.EmployerAccountId);
            var stopwatch = new Stopwatch();
            stopwatch.Start();


            await _messageSession.Publish(message);

            stopwatch.Stop();
            _telemetry.TrackDuration("BuildAccountProjection", stopwatch.Elapsed);
        }

    }
}