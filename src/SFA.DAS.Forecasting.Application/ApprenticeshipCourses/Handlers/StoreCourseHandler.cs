using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Handlers
{
    public class StoreCourseHandler
    {
        private readonly IApprenticeshipCourseDataService _dataService;
        private readonly ILog _logger;

        public StoreCourseHandler(IApprenticeshipCourseDataService dataService, ILog logger)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(ApprenticeshipCourse course)
        {
            _logger.Debug($"Storing apprenticeship course. Course: {course.ToJson()}");
            await _dataService.Store(course);
            _logger.Info($"Stored apprenticeship course. Course id: {course.Id}");
        }
    }
}