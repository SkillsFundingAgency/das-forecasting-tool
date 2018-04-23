using NUnit.Framework;
using SFA.DAS.Forecasting.Web.Orchestrators;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using AutoMoq;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Models.Projections;
using SFA.DAS.Forecasting.Domain.Estimations;
using SFA.DAS.HashingService;
using System.Collections.ObjectModel;

namespace SFA.DAS.Forecasting.Web.UnitTests
{
    [TestFixture]
    public class EstimationOrchestratorUnitTest
    {
        private AutoMoqer _autoMoq;
        private EstimationOrchestrator _estimationOrchestrator;
        private const string HashedAccountId = "VT6098";
        private const string EstimationName = "default";
        private const long AccountId = 12345;


        [SetUp]
        public void Setup()
        {
            _autoMoq = new AutoMoqer();

            var _model = new AccountEstimationModel
            {
                Id = Guid.NewGuid().ToString("N"),
                Apprenticeships = new List<VirtualApprenticeship>(),
                EmployerAccountId = AccountId,
                EstimationName = "default"
            };

            var p = _autoMoq.GetMock<IAccountEstimationProjection>();

            p.Setup(x => x.BuildProjections()).Verifiable();


            IList<AccountProjectionReadModel> projectionModel = new List<AccountProjectionReadModel>
                    {
                        new AccountProjectionReadModel
                        {
                            EmployerAccountId = 10000,
                            Month = 4,
                            Year = 2018,
                            FutureFunds = 15000m,
                            TotalCostOfTraining = 0m
                        }
                    };

            p.Setup(o => o.Projections)
                .Returns(new ReadOnlyCollection<AccountProjectionReadModel>(projectionModel));

            _autoMoq.SetInstance(_model);

            _autoMoq.GetMock<IHashingService>()
                    .Setup(o => o.DecodeValue(HashedAccountId))
                    .Returns(AccountId);

            _autoMoq.GetMock<IAccountEstimationRepository>()
                    .Setup(x => x.Get(It.IsAny<long>()))
                    .Returns(Task.FromResult(_autoMoq.Resolve<AccountEstimation>()));

            _autoMoq.GetMock<IAccountEstimationProjectionRepository>()
                  .Setup(x => x.Get(It.IsAny<AccountEstimation>()))
                  .Returns(Task.FromResult(p.Object));
            
            _estimationOrchestrator = _autoMoq.Resolve<EstimationOrchestrator>();
        }


        [Test]
        public async Task WhenRetrivingCostEstimationItShouldCallRequiredService()
        {
            //Act
            var estimationViewModel = await _estimationOrchestrator.CostEstimation(HashedAccountId, EstimationName, false);

            // Assert
            _autoMoq.Verify<IHashingService>(o => o.DecodeValue(HashedAccountId));
            _autoMoq.Verify<IAccountEstimationRepository>(o => o.Get(It.IsAny<long>()));
            _autoMoq.Verify<IAccountEstimationProjectionRepository>(o => o.Get(It.IsAny<AccountEstimation>()));
        }


        [Test]
        public async Task GivenAccountIdAndEstimationNameCostEstimationShouldReturnValidEstimationViewModel()
        {
            //Act
            var estimationViewModel = await _estimationOrchestrator.CostEstimation(HashedAccountId, EstimationName, false);

            // Assert
            estimationViewModel.Should().NotBeNull();
            estimationViewModel.CanFund.Should().BeTrue();
        }


        [Test]
        public async Task GetEstimationMethodShouldCallRequiredService()
        {
            //Act
            var estimationViewModel = await _estimationOrchestrator.GetEstimation(HashedAccountId);

            // Assert
            _autoMoq.Verify<IHashingService>(o => o.DecodeValue(HashedAccountId));
            _autoMoq.Verify<IAccountEstimationRepository>(o => o.Get(It.IsAny<long>()));
        }

        [Test]
        public async Task GetEstimationMethodShouldReturnAccountEstimation()
        {
            //Act
            var accountEstimation = await _estimationOrchestrator.GetEstimation(HashedAccountId);

            // Assert
            accountEstimation.Should().NotBeNull();
            accountEstimation.Should().BeOfType<AccountEstimation>();
        }

    }
}
