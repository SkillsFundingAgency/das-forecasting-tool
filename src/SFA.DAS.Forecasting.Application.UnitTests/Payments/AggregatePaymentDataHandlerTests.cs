using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Payments.Handlers;
using SFA.DAS.Forecasting.Domain.Payments.Services;
using SFA.DAS.Forecasting.Messages.Projections;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.UnitTests.Payments
{
    public class AggregatePaymentDataHandlerTests
    {
        private AggregatePaymentDataHandler _handler;
        private Mock<IEmployerPaymentDataSession> _paymentDataSession;
        private Mock<ILog> _logger;
        private const long ExpectedAccountId = 434535;
        private const int ExpectedCollectionYear = 2018;
        private const int ExpectedCollectionMonth = 10;

        [SetUp]
        public void Arrange()
        {
            _paymentDataSession = new Mock<IEmployerPaymentDataSession>();
            _logger = new Mock<ILog>();

            _handler = new AggregatePaymentDataHandler(_paymentDataSession.Object, _logger.Object);
        }

        [Test]
        public async Task Then_The_Payments_Are_Aggregated_For_The_Account_And_Period()
        {
            //Act
            await _handler.Handle(new AggregatePaymentDataCommand
            {
                EmployerAccountId = ExpectedAccountId,
                CollectionPeriodYear = ExpectedCollectionYear,
                CollectionPeriodMonth = ExpectedCollectionMonth
            });

            //Assert
            _paymentDataSession.Verify(x=>x.CalculatePaymentTotals(ExpectedAccountId, ExpectedCollectionYear, ExpectedCollectionMonth));
        }
    }
}
