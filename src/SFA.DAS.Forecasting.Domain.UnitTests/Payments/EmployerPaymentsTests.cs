using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Payments;
using SFA.DAS.Forecasting.Domain.Payments.Services;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Payments;

[TestFixture]
public class EmployerPaymentsTests
{
        
    [Test]
    public async Task Received_Recent_Payment_Returns_NotNull()
    {
        //Arrange
        var lastReceivedTime = DateTime.Now;
        var employerPaymentDataSession = new Mock<IEmployerPaymentDataSession>();
        employerPaymentDataSession
            .Setup(session => session.HasReceivedRecentPayment(It.IsAny<long>()))
            .Returns(Task.FromResult<bool>(lastReceivedTime != null));
            
        //Act
        var payments = new EmployerPayments(12345, employerPaymentDataSession.Object);
            
        //Assert
        Assert.AreEqual(true, await payments.HasReceivedRecentPayment());
    }

    [Test]
    public async Task Received_Recent_Payment_Returns_Null()
    {
        //Arrange
        var lastReceivedTime = DateTime.Now;
        var employerPaymentDataSession = new Mock<IEmployerPaymentDataSession>();
        employerPaymentDataSession
            .Setup(session => session.HasReceivedRecentPayment(It.IsAny<long>()))
            .Returns(Task.FromResult<bool>(lastReceivedTime == null));

        //Act
        var payments = new EmployerPayments(12345, employerPaymentDataSession.Object);

        //Assert
        Assert.AreEqual(false, await payments.HasReceivedRecentPayment());
    }
}