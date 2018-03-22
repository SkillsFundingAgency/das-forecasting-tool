using NUnit.Framework;
using SFA.DAS.Forecasting.Application.ApprenticeshipTraining;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Shared.Services;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Application.UnitTests
{
    [TestFixture]
    class TrainingProgrammeServiceTest
    {
        private TrainingProgrammeService _sut;

        [SetUp]
        public void SetUp()
        {
            var config = new StartupConfiguration(false)
            {
                ApprenticeshipApiBaseUrl = "http://das-prd-apprenticeshipinfoservice.cloudapp.net/"
            };

            _sut = new TrainingProgrammeService(config, new InMemoryCache());
        }

        [Test]
        public async Task GetAllTrainigPrograms()
        {
            var standards = await _sut.GetAllActiveStandards();
            var x = standards.Count();

        }
    }
}
