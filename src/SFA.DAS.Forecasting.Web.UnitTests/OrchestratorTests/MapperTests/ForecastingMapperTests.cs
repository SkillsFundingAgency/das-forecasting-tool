using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Forecasting.Models.Projections;
using SFA.DAS.Forecasting.Web.Orchestrators.Mappers;

namespace SFA.DAS.Forecasting.Web.UnitTests.OrchestratorTests.MapperTests;

[TestFixture]
public class ForecastingMapperTests
{
    private ForecastingMapper _mapper;
    private AccountProjectionModel _projectionModel;
    private IEnumerable<AccountProjectionModel> _models;
        
    [SetUp]
    public void SetUp()
    {
        _mapper = new ForecastingMapper();
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
                ExpiredFunds = 10
            };

        _models = new List<AccountProjectionModel>
        {
            _projectionModel
        };
    }

    [Test]
    public void ShouldMapFields()
    {
            
        var result = _mapper.MapProjections(_models);

        result.First().FundsIn.Should().Be(_projectionModel.LevyFundsIn+_projectionModel.TransferInCompletionPayments+_projectionModel.TransferInCostOfTraining);
        result.First().Balance.Should().Be(_projectionModel.FutureFunds);
        result.First().CoInvestmentEmployer.Should().Be(_projectionModel.CoInvestmentEmployer);
        result.First().CoInvestmentGovernment.Should().Be(_projectionModel.CoInvestmentGovernment);
        result.First().ExpiredFunds.Should().Be(_projectionModel.ExpiredFunds);
    }

    [Test]
    public void ShouldMapCostOfTraining()
    {
        _projectionModel.LevyFundedCostOfTraining = 200;
        _projectionModel.TransferOutCostOfTraining = 200;

        var result = _mapper.MapProjections(_models);

        result.First().CostOfTraining.Should().Be(_projectionModel.LevyFundedCostOfTraining + _projectionModel.TransferOutCostOfTraining);
    }

    [Test]
    public void ShouldMapTransferCost()
    {
        _projectionModel.LevyFundedCompletionPayments = 200;
        _projectionModel.TransferOutCompletionPayments = 200;

        var result = _mapper.MapProjections(_models);

        result.First().CompletionPayments.Should().Be(_projectionModel.LevyFundedCompletionPayments+ _projectionModel.TransferOutCompletionPayments);
    }

    [Test]
    public void Should_be_first_of_month()
    {
        _projectionModel.Month = 12;
        _projectionModel.Year = 1998;

        var result = _mapper.MapProjections(_models);

        result.First().Date.ToString("yyyy-MM-dd").Should().Be("1998-12-01");
    }
}