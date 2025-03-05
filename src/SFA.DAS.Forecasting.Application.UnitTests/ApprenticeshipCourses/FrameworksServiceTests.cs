using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.UnitTests.ApprenticeshipCourses;

public class FrameworksServiceTests
{
    private ApprenticeshipCourseFrameworkResponse _summaries;
    private Mock<IApiClient> _apiClient;
    
    [SetUp]
    public void SetUp()
    {
        _summaries = new ApprenticeshipCourseFrameworkResponse
        {
            Frameworks = new List<ApprenticeshipCourse>
            {
                new()
                {
                    Id = "test-123",
                    Level = 1,
                    Duration = 18,
                    CourseType = ApprenticeshipCourseType.Framework,
                    FundingCap = 10000,
                    Title = "Test course",
                    FundingPeriods = new List<FundingPeriod>()
                },
                new()
                {
                    Id = "test-789",
                    Level = 1,
                    Duration = 24,
                    CourseType = ApprenticeshipCourseType.Framework,
                    FundingCap = 10000,
                    Title = "Test course",
                    FundingPeriods = new List<FundingPeriod>()
                }
            }
        };
        _apiClient = new Mock<IApiClient>();
        _apiClient
            .Setup(x => x.Get<ApprenticeshipCourseFrameworkResponse>(It.IsAny<GetFrameworksApiRequest>()))
            .ReturnsAsync(_summaries);
    }

    [Test]
    public async Task Gets_All_Active_Frameworks()
    {
        var service = new FrameworksService(_apiClient.Object);
        var courses = await service.GetCourses();
        Assert.AreEqual(2, courses.Count);
        Assert.IsTrue(courses.All(course => course.Id == "test-123" || course.Id == "test-789"));
    }
}