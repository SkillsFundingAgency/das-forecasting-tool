using NUnit.Framework;
using SFA.DAS.Forecasting.Web.Orchestrators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using SFA.DAS.Forecasting.Application.Estimations.Service;
using Moq;
using AutoMoq;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Web.UnitTests
{
    [TestFixture]
    public class EstimationOrchestratorUnitTest
    {
        private AutoMoqer _autoMoq;
        private EstimationOrchestrator _estimationOrchestrator;
        private Mock<IAccountEstimationBuilderService> _accountEstimationBuilder;

        private const string  HashedAccountId = "VT6098";
        private const string EstimationName = "default";

        [SetUp]
        public void Setup()
        {

            _accountEstimationBuilder = new Mock<IAccountEstimationBuilderService>();
            _accountEstimationBuilder
                .Setup(o => o.CostBuildEstimations(HashedAccountId, EstimationName))
                .Returns(Task.FromResult(new AccountEstimation
                {
                    EstimationName = EstimationName,
                    Apprenticeships = new List<VirtualApprenticeship>
                    {
                       new VirtualApprenticeship
                       {
                           CourseTitle = "Plumbering Level 2",
                           TotalInstallments = 12,
                           MonthlyPayment = 1000.1m
                       }
                    },
                    TotalApprenticeshipCount = 2,
                    TotalCompletionPayment = 12000.12m,
                    TotalMonthlyPayment = 1000.1m,
                    Estimations = new List<AccountProjectionReadModel>
                    {
                        new AccountProjectionReadModel
                        {
                            EmployerAccountId = 10000,
                            Month = 4,
                            Year = 2018
                        }
                    }
                }));

            ///_autoMoq = new AutoMoqer();

            _estimationOrchestrator = new EstimationOrchestrator(_accountEstimationBuilder.Object);

        }

        [Test]
        public async Task GivenAccountIdAndEstimationNameShouldReturnEstimatioViewnModel()
        {
            //Act
            var estimationViewModel = await _estimationOrchestrator.CostEstimation(HashedAccountId, EstimationName);

            // Assert
            estimationViewModel.Should().NotBeNull();
        }



    }
}
