using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Projections.Handlers;
using SFA.DAS.Forecasting.Application.Projections.Services;
using SFA.DAS.Forecasting.Domain.Balance;
using SFA.DAS.Forecasting.Domain.Levy.Services;
using SFA.DAS.Forecasting.Domain.Projections;
using SFA.DAS.Forecasting.Domain.Projections.Services;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Application.UnitTests.Balance
{
    [TestFixture]
    public class GetAccountBalanceHandlerTests
    {
        private Mock<CurrentBalance> _balanceMock;
        private Mock<ICurrentBalanceRepository> _currentBalanceRepository;

        [SetUp]
        public void SetUp()
        {
            _balanceMock = new Mock<CurrentBalance>();
            _balanceMock.Setup(x => x.EmployerAccountId).Returns(12345);
            _balanceMock.Setup(x => x.Amount).Returns(10000);
            _balanceMock.Setup(x => x.RefreshBalance(It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Task.FromResult<bool>(true));
            _currentBalanceRepository = new Mock<ICurrentBalanceRepository>();
            _currentBalanceRepository.Setup(x => x.Get(It.IsAny<long>()))
                .Returns(Task.FromResult(_balanceMock.Object));
            _currentBalanceRepository.Setup(x => x.Store(It.IsAny<CurrentBalance>())).Returns(Task.CompletedTask);
        }

        private void HandleGenerateAccountBalance(ProjectionSource projection = ProjectionSource.LevyDeclaration)
        {
            var handler = new GetAccountBalanceHandler(new AccountProjectionService(
                new AccountProjectionRepository(_currentBalanceRepository.Object, 
                    Mock.Of<ILevyDataSession>(), 
                    Mock.Of<IAccountProjectionDataSession>())), _currentBalanceRepository.Object,
                Mock.Of<ILogger<GetAccountBalanceHandler>>());
            handler.Handle(new GenerateAccountProjectionCommand
            {
                EmployerAccountId = 12345,
                ProjectionSource = projection,
                StartPeriod = new Messages.Projections.CalendarPeriod { Month = 1, Year = 2018 }
            }).Wait();

        }

        [Test]
        public void Refreshes_Account_Balance()
        {
            HandleGenerateAccountBalance();
            _balanceMock
                .Verify(x => x.RefreshBalance(It.IsAny<bool>(), It.IsAny<bool>()), Times.Once());
        }

        [Test]
        public void Refreshes_Account_Balance_After_Payment()
        {
            HandleGenerateAccountBalance(ProjectionSource.PaymentPeriodEnd);
            _balanceMock
                .Verify(x => x.RefreshBalance(true, true), Times.Once());
        }

        [Test]
        public void Stores_Account_Balance_If_Refreshed_Ok()
        {
            HandleGenerateAccountBalance();
            _currentBalanceRepository
                .Verify(repo => repo.Store(It.Is<CurrentBalance>(bal => bal == _balanceMock.Object)), Times.Once());
        }

        [Test]
        public void Does_Not_Store_Account_Balance_If_Refresh_Failed()
        {
            _balanceMock
                .Setup(x => x.RefreshBalance(It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Task.FromResult<bool>(false));
            HandleGenerateAccountBalance();
            _currentBalanceRepository
                .Verify(repo => repo.Store(It.Is<CurrentBalance>(bal => bal == _balanceMock.Object)), Times.Never());
        }
    }
}