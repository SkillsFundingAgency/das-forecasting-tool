using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Interfaces;
using SFA.DAS.Forecasting.Infrastructure.Configuration;
using SFA.DAS.Forecasting.ReadModel.AccountProjections;
using SFA.DAS.Forecasting.Web.Orchestrators;
using SFA.DAS.Forecasting.Web.Orchestrators.Mappers;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Web.UnitTests
{
    [TestFixture]
    public class ForecastingOrcestratorTests
    {
        private Mock<IAccountProjectionDataService> _accountProjectionRepository;
        private ForecastingOrchestrator _orchestrator;

        private const string EmployerHash = "ABBA12";
        private const long EmployerId = 123456;

        [OneTimeSetUp]
        public void SetUp()
        {
            var hashingService = new Mock<IHashingService>();
            _accountProjectionRepository = new Mock<IAccountProjectionDataService>();
            hashingService.Setup(m => m.DecodeValue(EmployerHash)).Returns(EmployerId);
            _orchestrator = new ForecastingOrchestrator(
                hashingService.Object,
                _accountProjectionRepository.Object,
                Mock.Of<ForcastingApplicationConfiguration>(),
                Mock.Of<ILog>(),
                new Mapper());
        }
        [Test]
        public async Task Should_not_get_forecast_after_april_2019()
        {
            _accountProjectionRepository.Setup(m => m.Get(EmployerId))
                .Returns(Task.Run(() => GetList()));

            var result = await _orchestrator.Balance(EmployerHash);

            result.BalanceItemViewModels.ToList().Count().Should().Be(16);
            result.BalanceItemViewModels.Count(m => m.Date > DateTime.Parse("2019-04-01"))
                .Should().Be(0, "No balance rows after the first of April");
            result.BalanceItemViewModels.Count(m => m.Date == DateTime.Parse("2019-04-01"))
                .Should().Be(1, "Must create balance for the first of april");
        }

        private IEnumerable<AccountProjection> GetList()
        {
            var list = new List<AccountProjection>();
            var currentDate = DateTime.Parse("2018-01-01");
            while (currentDate < DateTime.Parse("2020-01-01"))
            {
                list.Add(new AccountProjection
                {
                    Year = currentDate.Year,
                    Month = (short)currentDate.Month,
                    FutureFunds = 200,
                    FundsIn = 500,
                    TotalCostOfTraning = 100,
                    CompletionPayments = 0
                });
                currentDate = currentDate.AddMonths(1);
            }
            return list;
        }
    }
}
