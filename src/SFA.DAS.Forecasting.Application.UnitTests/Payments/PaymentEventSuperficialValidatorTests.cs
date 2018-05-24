using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Payments.Validation;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Application.UnitTests.Payments
{
    [TestFixture]
    public class PaymentEventSuperficialValidatorTests
    {
        protected PaymentCreatedMessage PaymentCreatedMessage { get; set; }

        [SetUp]
        public void SetUp()
        {
            PaymentCreatedMessage = new PaymentCreatedMessage
            {
                EmployerAccountId = 1,
                SendingEmployerAccountId = 2,
                Amount = 100,
                ApprenticeshipId = 1,
                CollectionPeriod = new Application.Payments.Messages.NamedCalendarPeriod
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
                    CompletionStatus = 1,
                    MonthlyInstallment =  87.27m,
                    TotalInstallments = 12,
                    EndpointAssessorId = "EA-Id1",
                    ActualEndDate = DateTime.MinValue
                },
                ProviderName = "test provider",
                ApprenticeName = "test apprentice",
                CourseName = "test cource",
                CourseLevel = 1,
                Id = Guid.NewGuid().ToString("D"),
                Ukprn = 2,
                FundingSource = FundingSource.Levy
            };
        }

        [Test]
        public void Should_Pass_validation()
        {
            var validator = new PaymentEventSuperficialValidator();
            var result = validator.Validate(PaymentCreatedMessage);
            result.IsValid.Should().BeTrue();
        }

        [Test]
		public void Fails_If_Employer_Account_Id_Is_Not_Populated()
		{
			var validator = new PaymentEventSuperficialValidator();
			PaymentCreatedMessage.EmployerAccountId = 0;
			var result = validator.Validate(PaymentCreatedMessage);
            result.IsValid.Should().BeFalse();
        }

	    [Test]
	    public void Fails_If_Ukprn_Is_Negative()
	    {
		    var validator = new PaymentEventSuperficialValidator();
		    PaymentCreatedMessage.Ukprn = -1;
		    var result = validator.Validate(PaymentCreatedMessage);
            result.IsValid.Should().BeFalse();
        }

		[Test]
		public void Fails_If_Apprenticeship_Id_Is_Negative()
		{
			var validator = new PaymentEventSuperficialValidator();
			PaymentCreatedMessage.ApprenticeshipId = -1;
			var result = validator.Validate(PaymentCreatedMessage);
            result.IsValid.Should().BeFalse();
        }

	    [Test]
	    public void Fails_If_Amount_Is_Negative()
	    {
		    var validator = new PaymentEventSuperficialValidator();
		    PaymentCreatedMessage.Amount = -1;
		    var result = validator.Validate(PaymentCreatedMessage);
            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void Fails_If_Funding_Source_Is_LessThan_0()
        {
            var validator = new PaymentEventSuperficialValidator();
            PaymentCreatedMessage.FundingSource = (FundingSource)0;
            var result = validator.Validate(PaymentCreatedMessage);
            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void Fails_If_Funding_Source_Is_MoreThan_2()
        {
            var validator = new PaymentEventSuperficialValidator();
            PaymentCreatedMessage.FundingSource = (FundingSource)0;
            var result = validator.Validate(PaymentCreatedMessage);
            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void Fails_If_Earning_Details_Are_Null()
        {
            var validator = new PaymentEventSuperficialValidator();
            PaymentCreatedMessage.EarningDetails = null;
            var result = validator.Validate(PaymentCreatedMessage);
            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void Pass_If_Ids_are_equal_and_FundingSource_Levy()
        {
            var validator = new PaymentEventSuperficialValidator();
            PaymentCreatedMessage.SendingEmployerAccountId = PaymentCreatedMessage.EmployerAccountId;
            var result = validator.Validate(PaymentCreatedMessage);
            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void Failes_If_Ids_are_equal_and_FundingSource_Transefer()
        {
            var validator = new PaymentEventSuperficialValidator();
            PaymentCreatedMessage.SendingEmployerAccountId = PaymentCreatedMessage.EmployerAccountId;
            PaymentCreatedMessage.FundingSource = FundingSource.Transfer;
            var result = validator.Validate(PaymentCreatedMessage);
            result.IsValid.Should().BeFalse();
        }
    }
}