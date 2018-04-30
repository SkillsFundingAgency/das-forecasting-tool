using NUnit.Framework;
using SFA.DAS.Forecasting.Models.Payments;
using EmployerPayment = SFA.DAS.Forecasting.Domain.Payments.EmployerPayment;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Payments
{
    [TestFixture]
    public class EmployerPaymentTests
    {
        [Test]
        public void Stores_Payment_Info()
        {
            var id = "testId";
            var employerAccountId = 123456;
            var ukprn = 123456;
            var apprenticeshipId = 654321;
            var amount = 123m;
            var learnerId = 12;

            var payment = new EmployerPayment(new PaymentModel { EmployerAccountId = employerAccountId, ExternalPaymentId = id });
            payment.RegisterPayment(new PaymentModel
            {
                ExternalPaymentId = id,
                EmployerAccountId = employerAccountId,
                Amount = amount,
                ApprenticeshipId = apprenticeshipId,
                LearnerId = learnerId,
                ProviderId = ukprn
            });

            Assert.AreEqual(id, payment.ExternalPaymentId);
            Assert.AreEqual(employerAccountId, payment.EmployerAccountId);
            Assert.AreEqual(ukprn, payment.ProviderId);
            Assert.AreEqual(apprenticeshipId, payment.ApprenticeshipId);
            Assert.AreEqual(amount, payment.Amount);
        }
    }
}