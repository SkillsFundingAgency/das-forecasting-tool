using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Handlers;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.UnitTests.ApprenticeshipCourses;

[TestFixture]
public class StoreCourseHandlerTests
{
    [Test]
    public async Task Stores_The_Course()
    {
        var apprenticeshipCourseDataService = new Mock<IApprenticeshipCourseDataService>();
        var handler = new StoreCourseHandler(apprenticeshipCourseDataService.Object, Mock.Of<ILogger<StoreCourseHandler>>());
        var course = new ApprenticeshipCourse
        {
            Id = "course-1",
            Level = 1,
            Duration = 10,
            Title = "Test course",
            CourseType = ApprenticeshipCourseType.Standard,
            FundingCap = 10000
        };
        await handler.Handle(course);
        apprenticeshipCourseDataService
            .Verify(svc => svc.Store(It.Is<ApprenticeshipCourse>(storedCourse => storedCourse == course)));
    }
}