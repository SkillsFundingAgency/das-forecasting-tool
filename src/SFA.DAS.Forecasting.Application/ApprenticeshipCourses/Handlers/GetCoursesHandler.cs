using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Messages.ApprenticeshipCourses;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Handlers
{
    public interface IGetCoursesHandler
    {
        Task<List<ApprenticeshipCourse>> Handle(RefreshCourses message);
    }

    public class GetCoursesHandler : IGetCoursesHandler
    {
        private readonly IStandardsService _standardsService;
        private readonly IFrameworksService _frameworksService;
        private readonly ILogger<GetCoursesHandler> _logger;

        public GetCoursesHandler(
            IStandardsService standardsService,
            IFrameworksService frameworksService,
            ILogger<GetCoursesHandler> logger)
        {
            _standardsService = standardsService;
            _frameworksService = frameworksService;
            _logger = logger;
        }

        public async Task<List<ApprenticeshipCourse>> Handle(RefreshCourses message)
        {
            if (message.RequestTime < DateTime.Now.AddMinutes(-5))
            {
                _logger.LogWarning($"Received invalid request to get courses. Reason: Request was generated more than 5 minutes ago. Request time: {message.RequestTime}, current time: {DateTime.Now}.");
                return new List<ApprenticeshipCourse>();
            }
            var courses = new List<ApprenticeshipCourse>();

            if (message.CourseType.HasFlag(CourseType.Standards))
            {
                _logger.LogDebug($"Getting list of standard courses. RefrechTicks: {message.RequestTime.Ticks}");
                var standards = await _standardsService.GetCourses();
                courses.AddRange(standards);
            }

            if (message.CourseType.HasFlag(CourseType.Frameworks))
            {
                _logger.LogDebug($"Getting list of framework courses. RefrechTicks: {message.RequestTime.Ticks}");
                var frameworks = await _frameworksService.GetCourses();
                courses.AddRange(frameworks);
            }

            return courses;
        }
    }
}