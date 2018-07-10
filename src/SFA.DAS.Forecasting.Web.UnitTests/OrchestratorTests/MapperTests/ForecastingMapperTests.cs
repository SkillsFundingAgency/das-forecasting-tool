using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Forecasting.Models.Projections;
using SFA.DAS.Forecasting.Web.Orchestrators.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Forecasting.Web.UnitTests.OrchestratorTests.MapperTests
{
    [TestFixture]
    public class ForecastingMapperTests
    {
        private ForecastingMapper _mapper;
        private AccountProjectionMonth _projectionMonth;
        private IEnumerable<AccountProjectionMonth> _models;

        [SetUp]
        public void SetUp()
        {
            _mapper = new ForecastingMapper();
            _projectionMonth =
                new AccountProjectionMonth
                {
                    Id = 1,
                    EmployerAccountId = 14,
                    ProjectionCreationDate = DateTime.Today,
                    ProjectionGenerationType = ProjectionGenerationType.LevyDeclaration,
                    Month = (short)DateTime.Today.Month,
                    Year = DateTime.Today.Year,

                    LevyFundsIn = 100,
                    LevyFundedCostOfTraining = 100,
                    LevyFundedCompletionPayments = 100,


                    TransferInCostOfTraining = 100,
                    TransferOutCostOfTraining = 100,

                    TransferInCompletionPayments = 100,
                    TransferOutCompletionPayments = 100,

                    CommittedTransferCost = 100,
                    CommittedTransferCompletionCost = 100,
                    FutureFunds = 100,
                    CoInvestmentEmployer = 100,
                    CoInvestmentGovernment = 100,
                };

            _models = new List<AccountProjectionMonth>
            {
                _projectionMonth
            };
        }

        [Test]
        public void ShouldMapFields()
        {
            
            var result = _mapper.MapProjections(_models);

            result.First().FundsIn.Should().Be(_projectionMonth.LevyFundsIn+_projectionMonth.TransferInCompletionPayments+_projectionMonth.TransferInCostOfTraining);
            result.First().Balance.Should().Be(_projectionMonth.FutureFunds);
            result.First().CoInvestmentEmployer.Should().Be(_projectionMonth.CoInvestmentEmployer);
            result.First().CoInvestmentGovernment.Should().Be(_projectionMonth.CoInvestmentGovernment);
        }

        [Test]
        public void ShouldMapCostOfTraining()
        {
            _projectionMonth.LevyFundedCostOfTraining = 200;
            _projectionMonth.TransferOutCostOfTraining = 200;

            var result = _mapper.MapProjections(_models);

            result.First().CostOfTraining.Should().Be(_projectionMonth.LevyFundedCostOfTraining + _projectionMonth.TransferOutCostOfTraining);
        }

        [Test]
        public void ShouldMapTransferCost()
        {
            _projectionMonth.LevyFundedCompletionPayments = 200;
            _projectionMonth.TransferOutCompletionPayments = 200;

            var result = _mapper.MapProjections(_models);

            result.First().CompletionPayments.Should().Be(_projectionMonth.LevyFundedCompletionPayments+ _projectionMonth.TransferOutCompletionPayments);
        }

        [Test]
        public void Expired_funds_should_be_0()
        {
            var result = _mapper.MapProjections(_models);

            result.First().ExpiredFunds.Should().Be(0);
        }

        [Test]
        public void Should_be_first_of_month()
        {
            _projectionMonth.Month = 12;
            _projectionMonth.Year = 1998;

            var result = _mapper.MapProjections(_models);

            result.First().Date.ToString("yyyy-MM-dd").Should().Be("1998-12-01");
        }
    }
}
