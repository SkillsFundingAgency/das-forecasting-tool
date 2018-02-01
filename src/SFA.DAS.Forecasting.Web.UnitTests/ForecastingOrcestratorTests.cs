using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Entities;
using SFA.DAS.Forecasting.Domain.Interfaces;
using SFA.DAS.Forecasting.Web.Orchestrators;
using SFA.DAS.Forecasting.Web.Orchestrators.Mappers;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Web.UnitTests
{
    [TestFixture]
    public class ForecastingOrcestratorTests
    {
        private Mock<IBalanceRepository> _balanceRepository;
        private ForecastingOrchestrator _orchestrator;

        private const string EmployerHash = "ABBA12";
        private const long EmployerId = 123456;

        [OneTimeSetUp]
        public void SetUp()
        {
            var hashingService = new Mock<IHashingService>();
            _balanceRepository = new Mock<IBalanceRepository>();
            hashingService.Setup(m => m.DecodeValue(EmployerHash)).Returns(EmployerId);
            _orchestrator = new ForecastingOrchestrator(
                hashingService.Object,
                _balanceRepository.Object,
                Mock.Of<IApprenticeshipRepository>(),
                Mock.Of<ILog>(),
                new Mapper());
        }
        [Test]
        public async Task Should_not_get_forecast_after_april_2019()
        {
            _balanceRepository.Setup(m => m.GetBalanceAsync(EmployerId))
                .Returns(Task.Run(() => GetList()));

            var result = await _orchestrator.Balance(EmployerHash);

            result.BalanceItemViewModels.ToList().Count().Should().Be(16);
            result.BalanceItemViewModels.Count(m => m.Date > DateTime.Parse("2019-04-01"))
                .Should().Be(0, "No balance rows after the first of April");
            result.BalanceItemViewModels.Count(m => m.Date == DateTime.Parse("2019-04-01"))
                .Should().Be(1, "Must create balance for the first of april");
        }

        private IEnumerable<BalanceItem> GetList()
        {
            List<BalanceItem> list = new List<BalanceItem>();
            var currentDate = DateTime.Parse("2018-01-01");
            while (currentDate < DateTime.Parse("2020-01-01"))
            {
                list.Add(new BalanceItem
                {
                    Date = currentDate,
                    Balance = 200,
                    LevyCredit = 500,
                    CostOfTraining = 100,
                    ExpiredFunds = 0,
                    CompletionPayments = 0
                });
                currentDate = currentDate.AddMonths(1);
            }
            return list;
        }
    }
}
