using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMoq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.UnitTests.ApprenticeshipCourses
{
    public class FrameworksServiceTests
    {
        private AutoMoqer _moqer;
        private ApprenticeshipCourseResponse _summaries;

        [SetUp]
        public void SetUp()
        {
            _moqer = new AutoMoqer();
            _summaries = new ApprenticeshipCourseResponse
            {
                Standards = new List<ApprenticeshipCourse>
                {
                    new ApprenticeshipCourse
                    {
                        Id = "test-123",
                        Level = 1,
                        Duration = 18,
                        CourseType = ApprenticeshipCourseType.Framework,
                        FundingCap = 10000,
                        Title = "Test course",
                        FundingPeriods = new List<Models.Estimation.FundingPeriod>()
                    },
                    new ApprenticeshipCourse
                    {
                        Id = "test-789",
                        Level = 1,
                        Duration = 24,
                        CourseType = ApprenticeshipCourseType.Framework,
                        FundingCap = 10000,
                        Title = "Test course",
                        FundingPeriods = new List<Models.Estimation.FundingPeriod>()
                    }
                }
            };
            _moqer.GetMock<IApiClient>()
                .Setup(x => x.Get<ApprenticeshipCourseResponse>(It.IsAny<GetFrameworksApiRequest>()))
                .ReturnsAsync(_summaries);
        }

        [Test]
        public async Task Gets_All_Active_Frameworks()
        {
            var service = _moqer.Resolve<FrameworksService>();
            var courses = await service.GetCourses();
            Assert.AreEqual(2, courses.Count);
            Assert.IsTrue(courses.All(course => course.Id == "test-123" || course.Id == "test-789"));
        }
    }
}