using System;
using System.Threading.Tasks;
using AutoMoq;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.Forecasting.Application.Balance.Services;
using SFA.DAS.Forecasting.Application.Projections.Handlers;
using SFA.DAS.Forecasting.Domain.Balance;
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
            _accountDetailViewModel = new AccountDetailViewModel
            {
                Balance = 1122,
                AccountId = 12345,
                HashedAccountId = "MDPP87",
                TransferAllowance = 3344,
            };
            _moqer.GetMock<IAccountBalanceService>()
                .Setup(x => x.GetAccountBalance(It.IsAny<long>()))
                .Returns(Task.FromResult(_accountDetailViewModel));
            var repo = _moqer.GetMock<ICurrentBalanceRepository>();
            repo.Setup(x => x.Get(It.IsAny<long>()))
                .Returns(Task.FromResult(new CurrentBalance(new Models.Balance.Balance
                {
                    EmployerAccountId = 12345,
                    ReceivedDate = DateTime.Today.AddMonths(-1),
                    BalancePeriod = DateTime.Today.AddMonths(-1),
                })));
            repo.Setup(x => x.Store(It.IsAny<CurrentBalance>())).Returns(Task.Delay(1));
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
        public void Records_Account_Balance()
        {
            HandleGenerateAccountBalance();
            _moqer.GetMock<ICurrentBalanceRepository>()
                .Verify(x => x.Store(It.Is<CurrentBalance>(cb => cb.EmployerAccountId == 12345 &&
                                                                 cb.Amount == _accountDetailViewModel.Balance)));
        }
        [Test]
        public void Records_Transfer_Allowance()
        {
            HandleGenerateAccountBalance();
            _moqer.GetMock<ICurrentBalanceRepository>()
                .Verify(x => x.Store(It.Is<CurrentBalance>(cb => cb.EmployerAccountId == 12345 &&
                                                                 cb.TransferAllowance == _accountDetailViewModel.TransferAllowance)));
        }

        [Test]
        public void Records_Remaining_Transfer_Balance_Using_Transfer_Allowance_Value()
        {
            //TODO: Will need to change when EAS start sending a true remaining transfer balance
            HandleGenerateAccountBalance();
            _moqer.GetMock<ICurrentBalanceRepository>()
                .Verify(x => x.Store(It.Is<CurrentBalance>(cb => cb.EmployerAccountId == 12345 &&
                                                                 cb.RemainingTransferBalance == _accountDetailViewModel.TransferAllowance)));
        }
    }
}