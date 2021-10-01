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
        public async Task Received_Recent_Payment_Returns_NotNull()
        {
            //Arrange
            var lastReceivedTime = DateTime.Now;
            _moqer.GetMock<IEmployerPaymentDataSession>()
                .Setup(session => session.HasReceivedRecentPayment(It.IsAny<long>()))
                .Returns(Task.FromResult<bool?>(lastReceivedTime != null));
            
            //Act
            var payments = new EmployerPayments(12345, _moqer.GetMock<IEmployerPaymentDataSession>().Object);
            
            //Assert
            Assert.AreEqual(true, await payments.HasReceivedRecentPayment());
        }

        [Test]
        public async Task Received_Recent_Payment_Returns_Null()
        {
            //Arrange
            var lastReceivedTime = DateTime.Now;
            _moqer.GetMock<IEmployerPaymentDataSession>()
                .Setup(session => session.HasReceivedRecentPayment(It.IsAny<long>()))
                .Returns(Task.FromResult<bool?>(lastReceivedTime == null));

            //Act
            var payments = new EmployerPayments(12345, _moqer.GetMock<IEmployerPaymentDataSession>().Object);

            //Assert
            Assert.AreEqual(false, await payments.HasReceivedRecentPayment());
        }
    }
}