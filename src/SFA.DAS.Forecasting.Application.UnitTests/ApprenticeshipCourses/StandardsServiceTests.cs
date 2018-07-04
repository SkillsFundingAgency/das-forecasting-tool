using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMoq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;

namespace SFA.DAS.Forecasting.Application.UnitTests.ApprenticeshipCourses
{
    [TestFixture]
    public class StandardsServiceTests
    {
        private AutoMoqer _moqer;
        private List<StandardSummary> _summaries;

        [SetUp]
        public void SetUp()
        {
            _moqer = new AutoMoqer();
            _summaries = new List<StandardSummary>
            {
                new StandardSummary
                {
                    Id = "test-123",
                    Level = 1,
                    Duration = 18,
                    EffectiveFrom = new DateTime(2017,01,01),
                    EffectiveTo = null,
                    IsActiveStandard = true,
                    IsPublished = true,
                    CurrentFundingCap = 10000,
                    Title = "Test course",
                    FundingPeriods = new List<FundingPeriod>()
                },
                new StandardSummary
                {
                    Id = "test-456",
                    Level = 1,
                    Duration = 12,
                    EffectiveFrom = new DateTime(2017,01,01),
                    EffectiveTo = null,
                    IsActiveStandard = false,
                    IsPublished = true,
	                CurrentFundingCap = 10000,
                    Title = "Test inactive course",
                    FundingPeriods = new List<FundingPeriod>()
                },
                new StandardSummary
                {
                    Id = "test-789",
                    Level = 1,
                    Duration = 24,
                    EffectiveFrom = new DateTime(2018,01,01),
                    EffectiveTo = null,
                    IsActiveStandard = true,
                    IsPublished = true,
                    CurrentFundingCap = 10000,
                    Title = "Test course 2",
                    FundingPeriods = new List<FundingPeriod>()
                }
            };
            _moqer.GetMock<IStandardApiClient>()
                .Setup(x => x.GetAllAsync())
                .Returns(Task.FromResult<IEnumerable<StandardSummary>>(_summaries));
            _moqer.SetInstance<IStandardSummaryMapper>(new StandardSummaryMapper());
        }

        [Test]
        public async Task Gets_All_Active_Standards()
        {
            var service = _moqer.Resolve<StandardsService>();
            var courses = await service.GetCourses();
            Assert.AreEqual(2, courses.Count);
            Assert.IsTrue(courses.All(course => course.Id == "test-123" || course.Id == "test-789"));
        }
    }
}