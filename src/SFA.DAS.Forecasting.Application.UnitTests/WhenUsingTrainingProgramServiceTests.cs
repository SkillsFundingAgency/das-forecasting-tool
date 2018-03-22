using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.ApprenticeshipTraining;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Shared.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Application.UnitTests
{
    [TestFixture]
    public class WhenUsingTrainingProgramServiceTests
    {
        private ApprenticeshipApiConfig _config;
        private TrainingProgrammeService _sut;

        [SetUp]
        public void SetUp()
        {
            _config = new ApprenticeshipApiConfig();
            _sut = new TrainingProgrammeService(_config, new InMemoryCache());
        }

        [Test]
        [Ignore("External dependency - use to test the API")]
        public async Task ShouldCacheAllTheStandardsAfterCalling_GetAllActiveStandards()
        {
            var cache = new InMemoryCache();

            var c1 = await cache.Get<IEnumerable<ITrainingProgramme>>(_config.StandardsKey);

            var standards = await _sut.GetAllActiveStandards();

            var c2 = await cache.Get<IEnumerable<ITrainingProgramme>>(_config.StandardsKey);

            c1.Should().BeNull();
            c2.Any().Should().BeTrue();
        }

        [Test]
        [Ignore("External dependency - use to test the API")]
        public async Task ShouldCacheAllTheStandardsAndFrameworkAfterCalling_GetAllActiveCourses()
        {
            var cache = new InMemoryCache();

            var cs1 = await cache.Get<IEnumerable<ITrainingProgramme>>(_config.StandardsKey);
            var cf1 = await cache.Get<IEnumerable<ITrainingProgramme>>(_config.FrameworksKey);

            var standards = await _sut.GetAllActiveCourses();

            var cs2 = await cache.Get<IEnumerable<ITrainingProgramme>>(_config.StandardsKey);
            var cf2 = await cache.Get<IEnumerable<ITrainingProgramme>>(_config.FrameworksKey);

            cs1.Should().BeNull();
            cf1.Should().BeNull();
            cs2.Any().Should().BeTrue();
            cf2.Any().Should().BeTrue();
        }
    }
}
