using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Infrastructure.Persistence;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;

public interface IApprenticeshipCourseDataService
{
    List<ApprenticeshipCourse> GetAllStandardApprenticeshipCourses();
    List<ApprenticeshipCourse> GetAllApprenticeshipCourses();
    Task<ApprenticeshipCourse> GetApprenticeshipCourse(string courseId);
    Task Store(ApprenticeshipCourse course);
}

public class ApprenticeshipCourseDataService: IApprenticeshipCourseDataService
{
    private readonly IDocumentSession _documentSession;

    public ApprenticeshipCourseDataService(IDocumentSession documentSession)
    {
        _documentSession = documentSession ?? throw new ArgumentNullException(nameof(documentSession));
    }

    public List<ApprenticeshipCourse> GetAllStandardApprenticeshipCourses()
    {
        return _documentSession.CreateQuery<ApprenticeshipCourse>()
            .Where(course => course.CourseType == ApprenticeshipCourseType.Standard)
            .ToList();
    }

    public List<ApprenticeshipCourse> GetAllApprenticeshipCourses()
    {
        return _documentSession.CreateQuery<ApprenticeshipCourse>()
            .ToList();
    }

    public async Task<ApprenticeshipCourse> GetApprenticeshipCourse(string courseId)
    {
        return await _documentSession.Get<ApprenticeshipCourse>(courseId);
    }

    public async Task Store(ApprenticeshipCourse course)
    {
        await _documentSession.Store(course);
    }
}