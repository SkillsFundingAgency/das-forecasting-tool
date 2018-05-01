using System;
using System.Threading.Tasks;
using AutoMoq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Handlers;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.UnitTests.ApprenticeshipCourses
{
    [TestFixture]
    public class StoreCourseHandlerTests
    {
        private AutoMoqer _moqer;

        [SetUp]
        public void SetUp()
        {
            _moqer = new AutoMoqer();
            _moqer.GetMock<IApprenticeshipCourseDataService>()
                .Setup(svc => svc.Store(It.IsAny<ApprenticeshipCourse>()))
                .Returns(Task.CompletedTask);
        }

        [Test]
        public async Task Stores_The_Course()
        {
            var handler = _moqer.Resolve<StoreCourseHandler>();
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
            _moqer.GetMock<IApprenticeshipCourseDataService>()
                .Verify(svc => svc.Store(It.Is<ApprenticeshipCourse>(storedCourse => storedCourse == course)));
        }
    }
}