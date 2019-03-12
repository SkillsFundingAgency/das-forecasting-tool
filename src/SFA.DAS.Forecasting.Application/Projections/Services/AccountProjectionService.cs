using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Domain.Projections;
using SFA.DAS.Forecasting.Messages.Projections;
using SFA.DAS.Forecasting.Models.Projections;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.Projections.Services
{
    public interface IAccountProjectionService
    {
        Task<ProjectionSource> GetOriginalProjectionSource(long employerAccountId,
            ProjectionSource currentProjectionSource);
    }

    public class AccountProjectionService : IAccountProjectionService
    {
        private readonly IAccountProjectionRepository _accountProjectionRepository;
        private readonly ILog _logger;
        private readonly ITelemetry _telemetry;

        public AccountProjectionService(IAccountProjectionRepository accountProjectionRepository, ILog logger, ITelemetry telemetry)
        {
            _accountProjectionRepository = accountProjectionRepository ?? throw new ArgumentNullException(nameof(accountProjectionRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public async Task<ProjectionSource> GetOriginalProjectionSource(long employerAccountId, ProjectionSource currentProjectionSource)
        {
            _telemetry.AddEmployerAccountId(employerAccountId);
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var messageProjectionSource = currentProjectionSource;
            if (messageProjectionSource == ProjectionSource.Commitment)
            {
                var previousProjection = await _accountProjectionRepository.Get(employerAccountId);
                var projectionGenerationType = previousProjection?.FirstOrDefault()?.ProjectionGenerationType;
                if (projectionGenerationType != null)
                {
                    messageProjectionSource = projectionGenerationType == ProjectionGenerationType.LevyDeclaration
                        ? ProjectionSource.LevyDeclaration : ProjectionSource.PaymentPeriodEnd;
                }
            }
            stopwatch.Stop();
            _telemetry.TrackDuration("GetOriginalProjectionSource", stopwatch.Elapsed);

            return messageProjectionSource;
        }
    }

   
}
