using AutoMoq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Web.Orchestrators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace SFA.DAS.Forecasting.Web.UnitTests
{
    [TestFixture]
    public class EstimationOrchestratorUnitTest
    {
        private AutoMoqer _autoMoq;
        private EstimationOrchestrator _estimationOrchestrator;


        [SetUp]
        public void Setup()
        {
            _autoMoq = new AutoMoqer();

            _estimationOrchestrator = _autoMoq.Resolve<EstimationOrchestrator>();
        }


        [Test]
        public async Task GivenAccountIdAndEstimationNameShouldReturnEstimatioViewnModel()
        {
            // Arrange
            var accountId = "VT6098";
            var estimationName = "defualt";

            //Act
            var estimationViewModel = await _estimationOrchestrator.CostEstimation(accountId, estimationName);

            // Assert
            estimationViewModel.Should().NotBeNull();

        }

      

    }
}
