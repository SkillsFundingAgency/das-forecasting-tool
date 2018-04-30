using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Projections.Services;
using SFA.DAS.Forecasting.Models.Projections;
using SFA.DAS.Forecasting.Web.Orchestrators;
using SFA.DAS.Forecasting.Web.Orchestrators.Mappers;
using SFA.DAS.HashingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Web.UnitTests.OrchestratorTests
{
    [TestFixture]
    public class ForecastingOrchestratorTests
    {
        private ForecastingOrchestrator _sut;
        private Mock<IAccountProjectionReadModelDataService> _accountProjection;
        private Mock<IApplicationConfiguration> _applicationConfiguration;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();

            var hashingService = new Mock<IHashingService>();
            _accountProjection = new Mock<IAccountProjectionReadModelDataService>();
            _applicationConfiguration = new Mock<IApplicationConfiguration>();

            hashingService.Setup(m => m.DecodeValue("ABBA12"))
                .Returns(12345);

            _sut = new ForecastingOrchestrator(
                hashingService.Object,
                _accountProjection.Object,
                _applicationConfiguration.Object,
                new Mapper());
        }

        [Test]
        public async Task ShouldTake_48_months_projections()
        {
            SetUpProjections(48 + 10);

            var balance = await _sut.Balance("ABBA12");

            balance.BalanceItemViewModels.Count().Should().Be(48);
        }

        [Test]
        public async Task ShouldTake_48_months_csv_file()
        {
            SetUpProjections(48 + 10);

            var balanceCsv = await _sut.BalanceCsv("ABBA12");

            balanceCsv.Count().Should().Be(48);
        }

        [Test]
        public async Task Should_not_Take_months_after_2019_05_01()
        {
            var balanceMaxDate = new DateTime(2019, 05, 01);

            SetUpProjections(48 + 10);
            _applicationConfiguration.Setup(m => m.LimitForecast)
                .Returns(true);

            var balance = await _sut.Balance("ABBA12");

            balance.BalanceItemViewModels.All(m => m.Date < balanceMaxDate).Should().BeTrue();
            balance.BalanceItemViewModels.Count().Should().BeGreaterOrEqualTo(1);
            if (DateTime.Today >= balanceMaxDate)
                Assert.IsTrue(false, $"balanceMaxDate is out of date ({balanceMaxDate.ToString()}) and test can be removed");
        }

        [Test]
        public async Task Should_not_Take_months_after_2019_05_01_csv()
        {
            var balanceMaxDate = new DateTime(2019, 05, 01);
            SetUpProjections(48 + 10);
            _applicationConfiguration.Setup(m => m.LimitForecast)
                .Returns(true);

            var balanceCsv = await _sut.BalanceCsv("ABBA12");

            balanceCsv.All(m => DateTime.Parse(m.Date) < balanceMaxDate).Should().BeTrue();
            balanceCsv.Count().Should().BeGreaterOrEqualTo(1);
            if (DateTime.Today >= balanceMaxDate)
                Assert.IsTrue(false, $"balanceMaxDate is out of date ({balanceMaxDate.ToString()}) and test can be removed");
        }


        /// <summary>
        /// Setting up projections starting from last month. 
        /// </summary>
        /// <param name="count">Amount months to create starting from last month.</param>
        private void SetUpProjections(int count)
        {
            var projections = new List<AccountProjectionReadModel>();
            _fixture.AddManyTo(projections, count);

            var currentMonthDate = (new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)).AddMonths(-1);
            projections.ForEach(m =>
            {
                var date = DateTime.Parse(currentMonthDate.ToString());
                m.Month = (short)date.Month;
                m.Year = date.Year;

                currentMonthDate = currentMonthDate.AddMonths(1);
            });

            _accountProjection.Setup(m => m.Get(It.IsAny<long>()))
                .Returns(Task.FromResult(projections.AsEnumerable()));
        }
    }
}
