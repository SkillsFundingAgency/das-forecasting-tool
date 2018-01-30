using System;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Payments.Validation;

namespace SFA.DAS.Forecasting.Payments.UnitTests.Application
{
    [TestFixture]
    public class PaymentEventSuperficialValidatorTests
    {
        protected PaymentEvent PaymentEvent { get; set; }

        [SetUp]
        public void SetUp()
        {
            PaymentEvent = new PaymentEvent
            {
                EmployerAccountId = Guid.NewGuid().ToString("D"),
                Amount = 100,
                ApprenticeshipId = 1,
                CollectionPeriod = new CollectionPeriod
                {
                    Id = Guid.NewGuid().ToString("D"),
                    Month = 1,
                    Year = 2018
                },
                EarningDetails = new EarningDetails
                {
                    StartDate = DateTime.Today,
                    PlannedEndDate = DateTime.Today.AddMonths(14),
                    CompletionAmount = 240,
                    MonthlyInstallment =  87.27m,
                    TotalInstallments = 12
                },
                Id = Guid.NewGuid().ToString("D"),
                Ukprn = 2
            };
        }

		[Test]
		public void Fails_If_Employer_Account_Id_Is_Not_Populated()
		{
			var validator = new PaymentEventSuperficialValidator();
			PaymentEvent.EmployerAccountId = null;
			var result = validator.Validate(PaymentEvent);
			Assert.IsNotEmpty(result);
		}

	    [Test]
	    public void Fails_If_Ukprn_Is_Negative()
	    {
		    var validator = new PaymentEventSuperficialValidator();
		    PaymentEvent.Ukprn = -1;
		    var result = validator.Validate(PaymentEvent);
		    Assert.IsNotEmpty(result);
	    }

		[Test]
		public void Fails_If_Apprenticeship_Id_Is_Negative()
		{
			var validator = new PaymentEventSuperficialValidator();
			PaymentEvent.ApprenticeshipId = -1;
			var result = validator.Validate(PaymentEvent);
			Assert.IsNotEmpty(result);
		}

	    [Test]
	    public void Fails_If_Amount_Is_Negative()
	    {
		    var validator = new PaymentEventSuperficialValidator();
		    PaymentEvent.Amount = -1;
		    var result = validator.Validate(PaymentEvent);
		    Assert.IsNotEmpty(result);
	    }
	}
}