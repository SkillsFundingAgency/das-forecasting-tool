﻿using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Domain.Projections.Services;
using SFA.DAS.Forecasting.Models.Projections;
using SFA.DAS.Forecasting.Web.Orchestrators;
using SFA.DAS.Forecasting.Web.Orchestrators.Mappers;
using SFA.DAS.HashingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMoq;
using SFA.DAS.Forecasting.Domain.Balance.Services;
using SFA.DAS.Forecasting.Models.Balance;

namespace SFA.DAS.Forecasting.Web.UnitTests.OrchestratorTests
{
    [TestFixture]
    public class ForecastingOrchestratorTests
    {
        private AutoMoqer _moqer;
        private ForecastingOrchestrator _sut;
        private Mock<IAccountProjectionDataSession> _accountProjection;
        private Mock<IApplicationConfiguration> _applicationConfiguration;
        private Fixture _fixture;
        private BalanceModel _balance;

        [SetUp]
        public void SetUp()
        {
            _moqer = new AutoMoqer();
            _fixture = new Fixture();

            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _moqer.GetMock<IHashingService>()
                .Setup(m => m.DecodeValue("ABBA12"))
                .Returns(12345);
            _balance = new BalanceModel {EmployerAccountId = 12345, Amount = 50000, TransferAllowance = 5000, RemainingTransferBalance = 5000, UnallocatedCompletionPayments = 2000 };
            _moqer.GetMock<IBalanceDataService>()
                .Setup(x => x.Get(It.IsAny<long>()))
                .Returns(Task.FromResult(_balance));
            _accountProjection = _moqer.GetMock<IAccountProjectionDataSession>();
            _applicationConfiguration = _moqer.GetMock<IApplicationConfiguration>();
            _moqer.SetInstance(new Mapper());
            _sut = _moqer.Resolve<ForecastingOrchestrator>();
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
            var projections = new List<AccountProjectionModel>();
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
                .Returns(Task.FromResult(projections));
        }
    }
}