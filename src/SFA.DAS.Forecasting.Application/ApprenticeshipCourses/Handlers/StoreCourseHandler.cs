using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Application.Core;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Domain.Extensions;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Handlers;

public interface IStoreCourseHandler
{
    Task Handle(ApprenticeshipCourse course);
}

public class StoreCourseHandler : IStoreCourseHandler
{
    private readonly IApprenticeshipCourseDataService _dataService;
    private readonly ILogger<StoreCourseHandler> _logger;

    public StoreCourseHandler(IApprenticeshipCourseDataService dataService, ILogger<StoreCourseHandler> logger)
    {
        _dataService = dataService;
        _logger = logger;
    }

    public async Task Handle(ApprenticeshipCourse course)
    {
        _logger.LogDebug($"Storing apprenticeship course. Course: {course.ToJson()}");
        await _dataService.Store(course);
        _logger.LogDebug($"Stored apprenticeship course. Course id: {course.Id}");
    }
}