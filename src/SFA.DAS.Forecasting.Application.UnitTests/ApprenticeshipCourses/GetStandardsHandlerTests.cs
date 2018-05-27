using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMoq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Handlers;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Messages.ApprenticeshipCourses;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.UnitTests.ApprenticeshipCourses
{
    [TestFixture]
    public class GetStandardsHandlerTests
    {
        private AutoMoqer _moqer;

        [SetUp]
        public void SetUp()
        {
            _moqer = new AutoMoqer();
            _moqer.GetMock<IStandardsService>()
                .Setup(svc => svc.GetCourses())
                .Returns(Task.FromResult<List<ApprenticeshipCourse>>(new List<ApprenticeshipCourse> {new ApprenticeshipCourse
                {
                    Id = "course-1"
                }}));
        }

        [Test]
        public async Task Rejects_Requests_Generated_More_Than_5_Mins_In_The_Past()
        {
            var handler = _moqer.Resolve<GetStandardsHandler>();
            var courses = await handler.Handle(new RefreshCourses { RequestTime = DateTime.Now.AddMinutes(-10) });
            Assert.AreEqual(0, courses.Count);
        }

        [Test]
        public async Task Accepts_Requests_Generated_Less_Than_5_Mins_Ago()
        {
            var handler = _moqer.Resolve<GetStandardsHandler>();
            var courses = await handler.Handle(new RefreshCourses { RequestTime = DateTime.Now.AddMinutes(-4) });
            Assert.AreEqual(1, courses.Count);
        }

    }
}