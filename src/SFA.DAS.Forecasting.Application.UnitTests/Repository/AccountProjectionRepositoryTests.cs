using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Balance;
using SFA.DAS.Forecasting.Domain.Balance.Services;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Levy.Services;
using SFA.DAS.Forecasting.Domain.Projections;
using SFA.DAS.Forecasting.Domain.Projections.Services;
using SFA.DAS.Forecasting.Models.Balance;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Application.UnitTests.Repository
{
    [TestFixture]
    public class AccountProjectionRepositoryTests
    {
        private Mock<ICurrentBalanceRepository> _currentBalanceRepo;

        [SetUp]
        public void SetUp()
        {
            _currentBalanceRepo = new Mock<ICurrentBalanceRepository>();
            
            var balance = new BalanceModel
            {
                EmployerAccountId = 12345,
                BalancePeriod = DateTime.Today,
                Amount = 10000,
                TransferAllowance = 1000,
                RemainingTransferBalance = 1000
            };

            var commitments = new EmployerCommitments(12345, new EmployerCommitmentsModel
            {
                LevyFundedCommitments = new List<CommitmentModel>
                {
                    new CommitmentModel
                    {
                        EmployerAccountId = 12345,
                        PlannedEndDate = DateTime.Today.AddMonths(-2),
                        StartDate = DateTime.Today.AddMonths(-3),
                        NumberOfInstallments = 1,
                        CompletionAmount = 1000,
                        MonthlyInstallment = 80,
                        FundingSource = FundingSource.Levy
                    }
                }
            });

            _currentBalanceRepo.Setup(x => x.Get(It.IsAny<long>()))
                .ReturnsAsync(new CurrentBalance(balance, Mock.Of<IAccountBalanceService>(),commitments));
        }

        [Test]
        public async Task Positive_LevyAmount_Returns_Projection()
        {
            var levyDataSession = new Mock<ILevyDataSession>();
            levyDataSession.Setup(s => s.GetLatestLevyAmount(12345)).ReturnsAsync(400);

            var sut = new AccountProjectionRepository(_currentBalanceRepo.Object, levyDataSession.Object,
                Mock.Of<IAccountProjectionDataSession>());
           
            var projection = await sut.InitialiseProjection(12345);

            projection.Should().BeOfType<AccountProjection>();
            projection.EmployerAccountId.Should().Be(12345);
            projection.BuildLevyTriggeredProjections(DateTime.Now, 24);
            projection.Projections.Count.Should().Be(25);

        }

        [Test]
        public async Task Negative_LevyAmount_Returns_Projection()
        {
            var levyDataSession = new Mock<ILevyDataSession>();
            levyDataSession.Setup(s => s.GetLatestLevyAmount(12345)).ReturnsAsync(-400);
            levyDataSession.Setup(s => s.GetLatestPositiveLevyAmount(12345)).ReturnsAsync(400);

            var sut = new AccountProjectionRepository(_currentBalanceRepo.Object, levyDataSession.Object,
                Mock.Of<IAccountProjectionDataSession>());

            var projection = await sut.InitialiseProjection(12345);

            projection.Should().BeOfType<AccountProjection>();
            projection.EmployerAccountId.Should().Be(12345);
            projection.BuildLevyTriggeredProjections(DateTime.Now, 24);
            projection.Projections.Count.Should().Be(25);

        }
    }
}