using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Types.Models;
using SFA.DAS.Forecasting.Application.ExpiredFunds.Service;
using SFA.DAS.Forecasting.Application.Projections.Handlers;
using SFA.DAS.Forecasting.Application.Projections.Services;
using SFA.DAS.Forecasting.Core.Configuration;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Extensions;
using SFA.DAS.Forecasting.Domain.Projections;
using SFA.DAS.Forecasting.Messages.Projections;
using SFA.DAS.Forecasting.Models.Balance;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Projections;
using CalendarPeriod = SFA.DAS.EmployerFinance.Types.Models.CalendarPeriod;

namespace SFA.DAS.Forecasting.Application.UnitTests.Projections
{
    [TestFixture]
    public class BuildAccountProjectionHandlerTests
    {
        private BuildAccountProjectionHandler _handler;
        private Mock<IExpiredFundsService> _expiredFundsService;
        private Dictionary<EmployerFinance.Types.Models.CalendarPeriod, decimal> _expiredFunds;
        private Mock<IAccountProjectionRepository> _accountProjectionRepository;
        private AccountProjection _storedAccountProjection;

        [SetUp]
        public void Setup()
        {
            _expiredFunds = new Dictionary<EmployerFinance.Types.Models.CalendarPeriod, decimal>();

            _expiredFundsService = new Mock<IExpiredFundsService>();
            _expiredFundsService.Setup(x => x.GetExpiringFunds(It.IsAny<IList<AccountProjectionModel>>(),
                It.IsAny<long>(), It.IsAny<ProjectionSource>(), It.IsAny<DateTime>()))
                .ReturnsAsync(_expiredFunds);

            _accountProjectionRepository = new Mock<IAccountProjectionRepository>();
            _accountProjectionRepository.Setup(x => x.InitialiseProjection(It.IsAny<long>()))
                .ReturnsAsync(() => new AccountProjection(new Account(1,1,1,1,1), new EmployerCommitments(1, new EmployerCommitmentsModel())));

            _accountProjectionRepository.Setup(x => x.Store(It.IsAny<AccountProjection>()))
                .Callback((AccountProjection p) => _storedAccountProjection = p)
                .Returns(() => Task.CompletedTask);
               

            var config = new ForecastingJobsConfiguration();
            _handler = new BuildAccountProjectionHandler(
                _accountProjectionRepository.Object,
                Mock.Of<IAccountProjectionService>(),
                config,
                _expiredFundsService.Object, 
                Mock.Of<ILogger<BuildAccountProjectionHandler>>());
        }

        [Test]
        public async Task Expiry_Of_Funds_Prior_To_Today_Are_Omitted()
        {
            var command = new GenerateAccountProjectionCommand();

            var startDate = DateTime.Today.GetStartOfAprilOfFinancialYear();

            for (var i = 0; i < 12; i++)
            {
                var expiryDate = startDate.AddMonths(i);
                _expiredFunds.Add(new CalendarPeriod(expiryDate.Year, expiryDate.Month), 1000);
            }

            await _handler.Handle(command);

            _accountProjectionRepository.Verify(x => x.Store(It.IsAny<AccountProjection>()), Times.Once);

            Assert.IsTrue(_storedAccountProjection.Projections
                .Where(x => new DateTime(x.Year, x.Month, 19) < DateTime.Now)
                .All(x => x.ExpiredFunds == 0));
        }
    }
}
