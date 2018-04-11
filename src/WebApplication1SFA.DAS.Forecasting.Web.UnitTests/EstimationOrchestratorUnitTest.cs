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
using SFA.DAS.Forecasting.Domain.Estimations.Validation.VirtualApprenticeships;
using SFA.DAS.Forecasting.Domain.Shared.Validation;
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


            //_autoMoq.GetMock<IVirtualApprenticeshipValidator>()
            //    .Setup(x => x.Validate(It.IsAny<VirtualApprenticeship>()))
            //    .Returns(new List<ValidationResult>());



            //_autoMoq.GetMock<IAccountEstimationBuilderService>()
            //    .Setup(o => o.CostBuildEstimations(HashedAccountId, EstimationName))
            //    .Returns(Task.FromResult(new AccountEstimation
            //    {
            //        EstimationName = EstimationName,
            //        Apprenticeships = new List<VirtualApprenticeship>
            //        {
            //           new VirtualApprenticeship
            //           {
            //               CourseTitle = "Construction Building: Wood Occupations",
            //               Level  = 2,
            //               ApprenticesCount = 2,
            //               TotalInstallments = 18,
            //               StartDate = new DateTime(2018, 5, 1),
            //               TotalCost = 12000m,
            //               MonthlyPayment = 533.33m,
            //               CompletionPayment = 2400m,
            //           }
            //        },
            //        Estimations = new List<AccountProjectionReadModel>
            //        {
            //            new AccountProjectionReadModel
            //            {
            //                EmployerAccountId = 10000,
            //                Month = 4,
            //                Year = 2018,
            //                FutureFunds = 15000m,
            //                TotalCostOfTraining = 0m
            //            }
            //        }
            //    }));

            _estimationOrchestrator = _autoMoq.Resolve<EstimationOrchestrator>();
        }



        [Test]
        public async Task GivenAccountIdAndEstimationNameShouldReturnValidEstimationViewModel()
        {
            //Act
            var estimationViewModel = await _estimationOrchestrator.CostEstimation(HashedAccountId, EstimationName, false);

            // Assert
            estimationViewModel.Should().NotBeNull();
            estimationViewModel.CanFund.Should().BeTrue();

        }



    }
}
