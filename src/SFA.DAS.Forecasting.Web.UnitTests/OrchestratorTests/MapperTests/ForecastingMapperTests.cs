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
        private Mapper _mapper;
        private AccountProjectionModel _projectionModel;
        private IEnumerable<AccountProjectionModel> _models;

        [SetUp]
        public void SetUp()
        {
            _mapper = new Mapper();
            _projectionModel =
                new AccountProjectionModel
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

            _models = new List<AccountProjectionModel>
            {
                _projectionModel
            };
        }

        [Test]
        public void ShouldMapCostOfTraining()
        {
            _projectionModel.LevyFundedCostOfTraining = 200;
            _projectionModel.TransferOutCostOfTraining = 200;

            var result = _mapper.MapBalance(_models);

            result.First().CostOfTraining.Should().Be(_projectionModel.LevyFundedCostOfTraining + _projectionModel.TransferOutCostOfTraining);
        }

        [Test]
        public void ShouldMapTransferCost()
        {
            _projectionModel.LevyFundedCompletionPayments = 200;
            _projectionModel.TransferOutCompletionPayments = 200;

            var result = _mapper.MapBalance(_models);

            result.First().CompletionPayments.Should().Be(_projectionModel.LevyFundedCompletionPayments+ _projectionModel.TransferOutCompletionPayments);
        }
    }
}
