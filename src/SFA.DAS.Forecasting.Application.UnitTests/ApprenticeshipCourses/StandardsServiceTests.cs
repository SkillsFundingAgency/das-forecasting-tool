using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMoq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Apprenticeships.Api.Types;

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
                    MaxFunding = 10000,
                    Title = "Test course",
                }
            };
            _moqer.GetMock<IStandardApiClient>()
                .Setup(x => x.GetAllAsync())
                .Returns(Task.FromResult<IEnumerable<StandardSummary>>(_summaries));
        }
    }
}