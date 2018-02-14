using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Payments;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Payments
{
    [TestFixture]
    public class EmployerPaymentTests
    {
        private Mock<IEmployerPaymentRepository> _levyRepository;
        private EmployerPayment _service;

        [SetUp]
        public void SetUp()
        {
            _levyRepository = new Mock<IEmployerPaymentRepository>();
            
            _service = new EmployerPayment(_levyRepository.Object);
        }

		[Test]
		public async Task Stores_Valid_Payment()
		{
			var id = "testId";
			var employerAccountId = 123456;
			var ukprn = 123456;
			var apprenticeshipId = 654321;
			var amount = 123m;

			Payment payment = null;
			_levyRepository
				.Setup(m => m.StorePayment(It.IsAny<Payment>()))
				.Callback<Payment>(p => payment = p)
				.Returns(Task.Run(() => 1));

			await _service.AddPayment(new Payment
			{
				ExternalPaymentId = id,
				EmployerAccountId = employerAccountId,
				ProviderId = ukprn,
				ApprenticeshipId = apprenticeshipId,
				Amount = amount
			});

			Assert.AreEqual(id, payment.ExternalPaymentId);
			Assert.AreEqual(employerAccountId, payment.EmployerAccountId);
			Assert.AreEqual(ukprn, payment.ProviderId);
			Assert.AreEqual(apprenticeshipId, payment.ApprenticeshipId);
			Assert.AreEqual(amount, payment.Amount);

			_levyRepository.Verify(repo => repo.StorePayment(
				It.IsAny<Payment>()));
		}

	    [Test]
	    public void ReturnsCorrectLastTimeReceivedPayment()
	    {
		    var payments = new List<Payment>
		    {
				new Payment
			    {
					EmployerAccountId = 1,
				    ReceivedTime = DateTime.Now.AddMinutes(-2)
			    },
			    new Payment
			    {
					EmployerAccountId = 2,
				    ReceivedTime = DateTime.Now.AddMinutes(-1)
			    },
			    new Payment
			    {
					EmployerAccountId = 3,
				    ReceivedTime = DateTime.Now.AddMinutes(-3)
			    },
			    new Payment
			    {
					EmployerAccountId = 4,
				    ReceivedTime = DateTime.Now.AddMinutes(-5)
			    },
			};

		    var lastReceivedTime = _service.GetLastTimeReceivedPayment(payments);

			Assert.IsNotNull(lastReceivedTime);

		    Assert.AreEqual(lastReceivedTime, payments.FirstOrDefault(x => x.EmployerAccountId == 2).ReceivedTime);
	    }
	}
}