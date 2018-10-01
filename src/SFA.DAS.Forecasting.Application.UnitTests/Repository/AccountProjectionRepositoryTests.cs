using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMoq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Balance;
using SFA.DAS.Forecasting.Domain.Balance.Services;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Levy.Services;
using SFA.DAS.Forecasting.Domain.Projections;
using SFA.DAS.Forecasting.Models.Balance;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Application.UnitTests.Repository
{
    [TestFixture]
    public class AccountProjectionRepositoryTests
    {
        private AutoMoqer _moqer;

        [SetUp]
        public void SetUp()
        {
            _moqer = new AutoMoqer();
            

            var currentBalanceRepo = _moqer.GetMock<ICurrentBalanceRepository>();
            var mockBalance = _moqer.GetMock<CurrentBalance>();


            //_moqer.SetInstance(new Models.Balance.BalanceModel
            //{
            //    EmployerAccountId = 12345,
            //    ReceivedDate = DateTime.Today.AddMonths(-1),
            //    BalancePeriod = DateTime.Today.AddMonths(-1),
            //    TransferAllowance = 1
            //});
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

            mockBalance.Setup(x => x.EmployerAccountId).Returns(12345);
            mockBalance.Setup(x => x.Amount).Returns(10000);
            mockBalance.Setup(x => x.RefreshBalance(It.IsAny<bool>()))
                .ReturnsAsync(true);
           
            currentBalanceRepo.Setup(x => x.Get(It.IsAny<long>()))
                .ReturnsAsync(new CurrentBalance(balance,_moqer.GetMock<IAccountBalanceService>().Object,commitments));

            var levyDataSession = _moqer.GetMock<ILevyDataSession>();
            levyDataSession.Setup(s => s.GetLatestLevyAmount(12345)).ReturnsAsync(0);
            levyDataSession.Setup(s => s.GetLatestLevyAmount(12345)).ReturnsAsync(-400);

        }

        [Test]
        public async Task Positive_LevyAmount_Returns_Projection()
        {
            var levyDataSession = _moqer.GetMock<ILevyDataSession>();
            levyDataSession.Setup(s => s.GetLatestLevyAmount(12345)).ReturnsAsync(400);

            var sut = _moqer.Resolve<AccountProjectionRepository>();
           
            var projection = await sut.InitialiseProjection(12345);

            projection.Should().BeOfType<AccountProjection>();
            projection.EmployerAccountId.Should().Be(12345);
            projection.BuildLevyTriggeredProjections(DateTime.Now, 24);
            projection.Projections.Count.Should().Be(25);

        }

        [Test]
        public async Task Negative_LevyAmount_Returns_Projection()
        {
            var levyDataSession = _moqer.GetMock<ILevyDataSession>();
            levyDataSession.Setup(s => s.GetLatestLevyAmount(12345)).ReturnsAsync(-400);
            levyDataSession.Setup(s => s.GetLatestPositiveLevyAmount(12345)).ReturnsAsync(400);

            var sut = _moqer.Resolve<AccountProjectionRepository>();

            var projection = await sut.InitialiseProjection(12345);

            projection.Should().BeOfType<AccountProjection>();
            projection.EmployerAccountId.Should().Be(12345);
            projection.BuildLevyTriggeredProjections(DateTime.Now, 24);
            projection.Projections.Count.Should().Be(25);

        }
    }
}