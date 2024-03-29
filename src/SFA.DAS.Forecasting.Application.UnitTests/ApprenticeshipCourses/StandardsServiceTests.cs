﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.UnitTests.ApprenticeshipCourses
{
    [TestFixture]
    public class StandardsServiceTests
    {
        private ApprenticeshipCourseStandardsResponse _summaries;
        private Mock<IApiClient> _apiClient;

        [SetUp]
        public void SetUp()
        {
            _summaries =
                new ApprenticeshipCourseStandardsResponse
                {
                    Standards = new List<ApprenticeshipCourse>
                    {
                        new ApprenticeshipCourse
                        {
                            Id = "test-123",
                            Level = 1,
                            Duration = 18,
                            CourseType = ApprenticeshipCourseType.Standard,
                            FundingCap = 10000,
                            Title = "Test course",
                            FundingPeriods = new List<Models.Estimation.FundingPeriod>()
                        },
                        new ApprenticeshipCourse
                        {
                            Id = "test-789",
                            Level = 1,
                            Duration = 24,
                            CourseType = ApprenticeshipCourseType.Standard,
                            FundingCap = 10000,
                            Title = "Test course",
                            FundingPeriods = new List<Models.Estimation.FundingPeriod>()
                        }
                    }
                };
                
            _apiClient = new Mock<IApiClient>();
            _apiClient
                .Setup(x => x.Get<ApprenticeshipCourseStandardsResponse>(It.IsAny<GetStandardsApiRequest>()))
                .ReturnsAsync(_summaries);
        }

        [Test]
        public async Task Gets_All_Active_Standards()
        {
            var service = new StandardsService(_apiClient.Object);
            var courses = await service.GetCourses();
            Assert.AreEqual(2, courses.Count);
            Assert.IsTrue(courses.All(course => course.Id == "test-123" || course.Id == "test-789"));
        }
    }
}