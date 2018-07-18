using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Messages.ApprenticeshipCourses;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Handlers
{
    public class GetStandardsHandler
    {
        private readonly IStandardsService _standardsService;
        private readonly IAppInsightsTelemetry _logger;

        public GetStandardsHandler(IStandardsService standardsService, IAppInsightsTelemetry logger)
        {
            _standardsService = standardsService ?? throw new ArgumentNullException(nameof(standardsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<ApprenticeshipCourse>> Handle(RefreshCourses message)
        {
            if (message.RequestTime < DateTime.Now.AddMinutes(-5))
            {
                _logger.Warning("GetStandardsFunction", $"Received invalid request to get standards. Reason: Request was generated more than 5 minutes ago. Request time: {message.RequestTime}, current time: {DateTime.Now}.", "Handle");
                return new List<ApprenticeshipCourse>();
            }

            _logger.Info("GetStandardsFunction", "Getting list of standard courses.", "Handle");
            return await _standardsService.GetCourses();
        }
    }
}