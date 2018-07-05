using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Handlers
{
    public class StoreCourseHandler
    {
        private readonly IApprenticeshipCourseDataService _dataService;
        private readonly IAppInsightsTelemetry _logger;

        public StoreCourseHandler(IApprenticeshipCourseDataService dataService, IAppInsightsTelemetry logger)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(ApprenticeshipCourse course)
        {
            _logger.TrackEvent("StoreCourseFunction", $"Storing apprenticeship course. Course: {course.ToJson()}", "Handle");
            await _dataService.Store(course);
            _logger.TrackEvent("StoreCourseFunction", $"Stored apprenticeship course. Course id: {course.Id}", "Handle");
        }
    }
}