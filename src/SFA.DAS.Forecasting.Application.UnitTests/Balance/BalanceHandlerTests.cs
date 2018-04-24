using System;
using System.Threading.Tasks;
using AutoMoq;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.Forecasting.Application.Balance.Services;
using SFA.DAS.Forecasting.Application.Projections.Handlers;
using SFA.DAS.Forecasting.Domain.Balance;
using SFA.DAS.Forecasting.Domain.Balance.Services;
using SFA.DAS.Forecasting.Messages.Projections;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Application.UnitTests.Balance
{
    [TestFixture]
    public class GetAccountBalanceHandlerTests
    {
        private AutoMoqer _moqer;
        private AccountDetailViewModel _accountDetailViewModel;

        [SetUp]
        public void SetUp()
        {
            _moqer = new AutoMoqer();

            _moqer.SetInstance(new Models.Balance.BalanceModel
            {
                EmployerAccountId = 12345,
                ReceivedDate = DateTime.Today.AddMonths(-1),
                BalancePeriod = DateTime.Today.AddMonths(-1),
            });

            var mockBalance = _moqer.GetMock<CurrentBalance>();
            mockBalance.Setup(x => x.EmployerAccountId).Returns(12345);
            mockBalance.Setup(x => x.Amount).Returns(10000);
            mockBalance.Setup(x => x.RefreshBalance())
                .Returns(Task.FromResult<bool>(true));
            var repo = _moqer.GetMock<ICurrentBalanceRepository>();
            repo.Setup(x => x.Get(It.IsAny<long>()))
                .Returns(Task.FromResult(mockBalance.Object));
            repo.Setup(x => x.Store(It.IsAny<CurrentBalance>())).Returns(Task.CompletedTask);
        }

        private void HandleGenerateAccountBalance()
        {
            var handler = _moqer.Resolve<GetAccountBalanceHandler>();
            handler.Handle(new GenerateAccountProjectionCommand
            {
                EmployerAccountId = 12345,
                ProjectionSource = ProjectionSource.PaymentPeriodEnd,
                StartPeriod = new CalendarPeriod { Month = 1, Year = 2018 }
            }).Wait();

        }

        [Test]
        public void Refreshes_Account_Balance()
        {
            HandleGenerateAccountBalance();
            _moqer.GetMock<CurrentBalance>()
                .Verify(x => x.RefreshBalance(), Times.Once());
        }

        [Test]
        public void Stores_Account_Balance_If_Refreshed_Ok()
        {
            HandleGenerateAccountBalance();
            _moqer.GetMock<ICurrentBalanceRepository>()
                .Verify(repo => repo.Store(It.Is<CurrentBalance>(bal => bal == _moqer.GetMock<CurrentBalance>(MockBehavior.Loose).Object)), Times.Once());
        }

        [Test]
        public void Does_Not_Store_Account_Balance_If_Refresh_Failed()
        {
            _moqer.GetMock<CurrentBalance>()
                .Setup(x => x.RefreshBalance())
                .Returns(Task.FromResult<bool>(false));
            HandleGenerateAccountBalance();
            _moqer.GetMock<ICurrentBalanceRepository>()
                .Verify(repo => repo.Store(It.Is<CurrentBalance>(bal => bal == _moqer.GetMock<CurrentBalance>(MockBehavior.Loose).Object)), Times.Never());
        }

    }
}