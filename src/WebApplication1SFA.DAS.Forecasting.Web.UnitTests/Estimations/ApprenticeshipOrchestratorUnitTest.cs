using NUnit.Framework;
using SFA.DAS.Forecasting.Web.Orchestrators;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using AutoMoq;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Domain.Estimations;
using SFA.DAS.HashingService;
using System.Collections.ObjectModel;
using SFA.DAS.Forecasting.Application.Estimations.Services;
using SFA.DAS.Forecasting.Web.Orchestrators.Estimations;
using SFA.DAS.Forecasting.Web.ViewModels;
using SFA.DAS.Forecasting.Web.Orchestrators.Exceptions;

namespace SFA.DAS.Forecasting.Web.UnitTests.Estimations
{
    [TestFixture]
    public class ApprenticeshipOrchestratorUnitTest
    {

        private ApprenticeshipOrchestrator _apprenticeshipOrchestrator;

        private AutoMoqer _autoMoq;

        private const string HashedAccountId = "VT6098";
        private const string EstimationName = "default";
        private const long AccountId = 12345;
        private const string VirtualApprenticeshipId = "10000ABC";

        [SetUp]
        public void Setup()
        {
            _autoMoq = new AutoMoqer();

            var _model = new AccountEstimationModel
            {
                Id = Guid.NewGuid().ToString("N"),
                Apprenticeships = new List<VirtualApprenticeship>
                {
                    new VirtualApprenticeship
                    {
                        Id = VirtualApprenticeshipId,
                        CourseTitle = "Wood work",
                        Level = 2,
                        ApprenticesCount = 5
                    }
                },
                EmployerAccountId = AccountId,
                EstimationName = "default"
            };
            _autoMoq.SetInstance(_model);

            _autoMoq.GetMock<IHashingService>()
                    .Setup(o => o.DecodeValue(HashedAccountId))
                    .Returns(AccountId);

            _autoMoq.GetMock<IAccountEstimationRepository>()
                    .Setup(x => x.Get(It.IsAny<long>()))
                    .Returns(Task.FromResult(_autoMoq.Resolve<AccountEstimation>()));

            _apprenticeshipOrchestrator = _autoMoq.Resolve<ApprenticeshipOrchestrator>();

        }

        [Test]
        public async Task ConfirmRemovalShouldReturnValidViewModel()
        {
            //Act
            var confirmRemovalViewModel = await _apprenticeshipOrchestrator.GetVirtualApprenticeshipsForRemoval(HashedAccountId, VirtualApprenticeshipId, EstimationName);
            // Assert
            _autoMoq.Verify<IHashingService>(o => o.DecodeValue(HashedAccountId));
            _autoMoq.Verify<IAccountEstimationRepository>(o => o.Get(It.IsAny<long>()));

            confirmRemovalViewModel.Should().NotBeNull();
            confirmRemovalViewModel.Should().BeOfType<RemoveApprenticeshipViewModel>();
        }

        [Test]
        public void ConfirmRemovalShouldThrowExceptionIfApprenticeshipIsAlreadyRemoved()
        {
            // Arrange
            _autoMoq.Resolve<AccountEstimationModel>().Apprenticeships = null;

            //Act
            Func<Task> action = async () => { await _apprenticeshipOrchestrator.GetVirtualApprenticeshipsForRemoval(HashedAccountId, VirtualApprenticeshipId, EstimationName); };

            action.ShouldThrow<ApprenticeshipAlreadyRemovedException>();
        }

    }
}
