using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Payments.Aggregates;
using SFA.DAS.Forecasting.Domain.Payments.Entities;
using SFA.DAS.Forecasting.Domain.Payments.Repositories;

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
	        var employerAccountId = "123456";
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
	            Id = id,
				EmployerAccountId = employerAccountId,
				Ukprn = ukprn,
				ApprenticeshipId = apprenticeshipId,
				Amount = amount
            });

            Assert.AreEqual(id, payment.Id);
            Assert.AreEqual(employerAccountId, payment.EmployerAccountId);
            Assert.AreEqual(ukprn, payment.Ukprn);
            Assert.AreEqual(apprenticeshipId, payment.ApprenticeshipId);
            Assert.AreEqual(amount, payment.Amount);
			
            _levyRepository.Verify(repo => repo.StorePayment(
                    It.IsAny<Payment>()));
        }
    }
}