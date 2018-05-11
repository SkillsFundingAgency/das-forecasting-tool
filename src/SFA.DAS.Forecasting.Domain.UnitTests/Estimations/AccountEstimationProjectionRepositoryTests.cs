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

namespace SFA.DAS.Forecasting.Domain.UnitTests.Estimations
{
    [TestFixture]
    public class AccountEstimationProjectionRepositoryTests
    {

        [Test]
        public async Task Apprenticeships_from_estimation_should_be_transfer_from_sender()
        {
            // Arrange
            var balanceRepository = new Mock<ICurrentBalanceRepository>();
            var balanceService = new Mock<IAccountBalanceService>();

            var balance = new CurrentBalance(new BalanceModel(), balanceService.Object);

            balanceRepository.Setup(m => m.Get(It.IsAny<long>()))
                .Returns(Task.FromResult(balance));
            var sut = new AccountEstimationProjectionRepository(balanceRepository.Object);

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
            var virtualApprenticeshipValidator = new Mock<IVirtualApprenticeshipValidator>();

            // Act
            var accountEstimation = new AccountEstimation(accountEstimationModel, virtualApprenticeshipValidator.Object);
            var result = await sut.Get(accountEstimation);

            result.BuildProjections();

            // Assert
            result.Projections.Count().Should().Be(26);
            result.Projections.All(m => m.TransferOutCompletionPayments > 0 || m.TransferOutTotalCostOfTraining > 0).Should().BeTrue();

            result.Projections.Any(m => m.TransferInTotalCostOfTraining > 0 || m.TransferInTotalCostOfTraining > 0).Should().BeFalse();
            result.Projections.Any(m => m.CompletionPayments > 0 ).Should().BeFalse();
        }
    }
}
