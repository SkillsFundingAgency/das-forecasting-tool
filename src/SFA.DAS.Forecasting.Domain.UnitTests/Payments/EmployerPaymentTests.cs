using System;
using NUnit.Framework;
using SFA.DAS.Forecasting.Models.Payments;
using EmployerPayment = SFA.DAS.Forecasting.Domain.Payments.EmployerPayment;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Payments;

[TestFixture]
public class EmployerPaymentTests
{
    private const string  Id = "testId";
    private const long EmployerAccountId = 123456;
    private const long Ukprn = 123456;
    private const long ApprenticeshipId = 654321;
    private const decimal Amount = 123m;
    private const long LearnerId = 12;

    [Test]
    public void Stores_Payment_Info()
    {
        
        var payment = new EmployerPayment(new PaymentModel { EmployerAccountId = EmployerAccountId, ExternalPaymentId = "TESTID" });
        payment.RegisterPayment(new PaymentModel
        {
            ExternalPaymentId = Id,
            EmployerAccountId = EmployerAccountId,
            Amount = Amount,
            ApprenticeshipId = ApprenticeshipId,
            LearnerId = LearnerId,
            ProviderId = Ukprn
        });

        Assert.IsTrue(Id.Equals(payment.ExternalPaymentId, StringComparison.CurrentCultureIgnoreCase));
        Assert.AreEqual(EmployerAccountId, payment.EmployerAccountId);
        Assert.AreEqual(Ukprn, payment.ProviderId);
        Assert.AreEqual(ApprenticeshipId, payment.ApprenticeshipId);
        Assert.AreEqual(Amount, payment.Amount);
    }

    [Test]
    public void Then_An_InvalidOperationException_Is_Thrown_When_The_Ids_Do_Not_Match()
    {
        var payment = new EmployerPayment(new PaymentModel { EmployerAccountId = EmployerAccountId, ExternalPaymentId = "DDDD" });
            
        Assert.Throws<InvalidOperationException>(() =>
        {
            payment.RegisterPayment(new PaymentModel
            {
                ExternalPaymentId = Id,
                EmployerAccountId = EmployerAccountId,
                Amount = Amount,
                ApprenticeshipId = ApprenticeshipId,
                LearnerId = LearnerId,
                ProviderId = Ukprn
            });
        });
    }
}