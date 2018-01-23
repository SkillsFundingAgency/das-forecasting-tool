using System;
using NUnit.Framework;
using SFA.DAS.Forecasting.Payments.Application.Messages;

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
                Uln = 1,
                Id = Guid.NewGuid().ToString("D"),
                Ukprn = 2
            };
        }

        [Test]
        public void Rejects_Invalid_Employer_Account_Id()
        {
            
        }
    }
}