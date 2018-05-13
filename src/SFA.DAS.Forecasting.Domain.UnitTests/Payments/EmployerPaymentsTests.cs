using System;
using System.Threading.Tasks;
using AutoMoq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Payments;
using SFA.DAS.Forecasting.Domain.Payments.Services;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Payments
{
    [TestFixture]
    public class EmployerPaymentsTests
    {
        private AutoMoqer _moqer;

        [SetUp]
        public void SetUp()
        {
            _moqer = new AutoMoqer();
        }

        [Test]
        public async Task Returns_Last_Received_Time()
        {
            var lastReceivedTime = DateTime.Now;
            _moqer.GetMock<IEmployerPaymentDataSession>()
                .Setup(session => session.GetLastReceivedTime(It.IsAny<long>()))
                .Returns(Task.FromResult<DateTime?>(lastReceivedTime));
            var payments = new EmployerPayments(12345, _moqer.GetMock<IEmployerPaymentDataSession>().Object);
            Assert.AreEqual(lastReceivedTime, await payments.GetLastTimeReceivedPayment());
        }
    }
}