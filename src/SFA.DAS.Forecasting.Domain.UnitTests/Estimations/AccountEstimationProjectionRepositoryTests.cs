using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Balance;
using SFA.DAS.Forecasting.Domain.Balance.Services;
using SFA.DAS.Forecasting.Domain.Estimations;
using SFA.DAS.Forecasting.Domain.Estimations.Validation.VirtualApprenticeships;
using SFA.DAS.Forecasting.Models.Balance;
using SFA.DAS.Forecasting.Models.Estimation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Projections.Services;
using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Estimations
{
    public class AccountEstimationProjectionRepositoryTests
    {
        private Mock<ICurrentBalanceRepository> _balanceRepository;
        private Mock<IAccountBalanceService> _balanceService;
        private Mock<IAccountProjectionDataSession> _accountProjectionRepository;
        private Mock<IVirtualApprenticeshipValidator> _virtualApprenticeshipValidator;

        private AccountEstimationProjectionRepository _sut;

        [SetUp]
        public void Arrange()
        {
            _balanceRepository = new Mock<ICurrentBalanceRepository>();
            _balanceService = new Mock<IAccountBalanceService>();
            _accountProjectionRepository = new Mock<IAccountProjectionDataSession>();
            _virtualApprenticeshipValidator = new Mock<IVirtualApprenticeshipValidator>();

            var balance = new CurrentBalance(new BalanceModel{RemainingTransferBalance = 1000, TransferAllowance = 15000}, _balanceService.Object);
            _balanceRepository.Setup(m => m.Get(It.IsAny<long>())).ReturnsAsync(balance);

            _accountProjectionRepository.Setup(x => x.Get(It.IsAny<long>())).ReturnsAsync(new List<AccountProjectionModel>());

            _sut = new AccountEstimationProjectionRepository(_balanceRepository.Object, _accountProjectionRepository.Object);
        }

        [Test]
        public async Task Apprenticeships_from_estimation_should_be_transfer_from_sender()
        {
            // Arrange
            var accountEstimationModel = new AccountEstimationModel
            {
                EmployerAccountId = 12345,
                Apprenticeships = new List<VirtualApprenticeship>
                {
                    new VirtualApprenticeship
                    {
                        ApprenticesCount = 2,
                        CourseId = "7",
                        CourseTitle = "Josevi driver",
                        Id = "1",
                        Level = 10,
                        StartDate = DateTime.Today,
                        TotalCompletionAmount = 2000,
                        TotalCost = 10000,
                        TotalInstallmentAmount = 120,
                        TotalInstallments = 12,
                        FundingSource = Models.Payments.FundingSource.Transfer
                    },
                    new VirtualApprenticeship
                    {
                        ApprenticesCount = 2,
                        CourseId = "7",
                        CourseTitle = "Josevi driver",
                        Id = "2",
                        Level = 10,
                        StartDate = DateTime.Today.AddMonths(13),
                        TotalCompletionAmount = 100,
                        TotalCost = 500,
                        TotalInstallmentAmount = 300,
                        TotalInstallments = 12,
                        FundingSource = 0
                    }
                }
            };

            // Act
            var accountEstimation =
                new AccountEstimation(accountEstimationModel, _virtualApprenticeshipValidator.Object);
            var result = await _sut.Get(accountEstimation);
            result.BuildProjections();

            // Assert
            result.Projections.Count().Should().Be(27);
            result.Projections.Take(result.Projections.Count-1).All(m => m.TransferOutCompletionPayments > 0 || m.TransferOutTotalCostOfTraining > 0)
                .Should().BeTrue();
            result.Projections.Take(result.Projections.Count - 1).Any(m => m.TransferInTotalCostOfTraining > 0 || m.TransferInTotalCostOfTraining > 0)
                .Should().BeFalse();
            result.Projections.Take(result.Projections.Count - 1).Any(m => m.CompletionPayments > 0).Should().BeFalse();
        }

        [Test]
        public async Task Then_The_Actual_Projections_Are_Taken_From_The_Repository()
        {
            //Arrange
            var expectedEmployerAccountId = 4332255;
            var accountProjetionModel =
                new List<AccountProjectionModel>
                {
                    new AccountProjectionModel
                    {
                        Month = (short)(DateTime.Today.Month + 1),
                        Year = DateTime.Today.Year,
                        TransferOutTotalCostOfTraining = 10m,
                        TransferOutCompletionPayments = 0m
                    }
                };
            _accountProjectionRepository.Setup(x => x.Get(It.IsAny<long>())).ReturnsAsync(accountProjetionModel);
            var accountEstimationModel = new AccountEstimationModel {EmployerAccountId = expectedEmployerAccountId, Apprenticeships = new List<VirtualApprenticeship>
            {
                new VirtualApprenticeship
                {
                    ApprenticesCount = 1,
                    CourseId = "7",
                    CourseTitle = "Test Tester",
                    Id = "1",
                    Level = 10,
                    StartDate = DateTime.Today,
                    TotalCompletionAmount = 2000,
                    TotalCost = 10000,
                    TotalInstallmentAmount = 120,
                    TotalInstallments = 3,
                    FundingSource = Models.Payments.FundingSource.Transfer
                }
            }
            };

            //Act
            var actual = await _sut.Get(new AccountEstimation(accountEstimationModel, _virtualApprenticeshipValidator.Object));
            actual.BuildProjections();

            //Assert
            _accountProjectionRepository.Verify(x => x.Get(expectedEmployerAccountId), Times.Once());
            Assert.AreEqual(5, actual.Projections.Count);
            Assert.AreEqual(870, actual.Projections[1].FutureFunds);
        }
    }
}