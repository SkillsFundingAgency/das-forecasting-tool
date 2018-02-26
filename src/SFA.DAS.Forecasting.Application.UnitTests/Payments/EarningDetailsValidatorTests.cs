using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Payments.Validation;

namespace SFA.DAS.Forecasting.Application.UnitTests.Payments
{
    [TestFixture]
    public class EarningDetailsValidatorTests
    {
        protected EarningDetails EarningDetails { get; set; }

        [SetUp]
        public void SetUp()
        {
            EarningDetails = new EarningDetails()
            {
                StartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.AddMonths(14),
                CompletionAmount = 240,
                MonthlyInstallment =  87.27m,
                TotalInstallments = 12
            };
        }

	    [Test]
	    public void Fails_If_Start_Date_Is_Invalid()
	    {
		    var validator = new EarningDetailsSuperficialValidator();
		    EarningDetails.StartDate = new DateTime(0001, 01, 01);
		    var result = validator.Validate(EarningDetails);
            result.IsValid.Should().BeFalse();
        }

		[Test]
		public void Fails_If_Planned_End_Date_Is_Invalid()
		{
			var validator = new EarningDetailsSuperficialValidator();
			EarningDetails.PlannedEndDate = new DateTime(0001, 01, 01);
			var result = validator.Validate(EarningDetails);
            result.IsValid.Should().BeFalse();
        }

	    [Test]
	    public void Fails_If_Actual_End_Date_Is_Invalid()
	    {
		    var validator = new EarningDetailsSuperficialValidator();
		    EarningDetails.ActualEndDate = new DateTime(0001, 01, 01);
		    var result = validator.Validate(EarningDetails);
            result.IsValid.Should().BeFalse();
        }

		[Test]
	    public void Fails_If_Completion_Amount_Is_Negative()
	    {
		    var validator = new EarningDetailsSuperficialValidator();
		    EarningDetails.CompletionAmount = -1;
		    var result = validator.Validate(EarningDetails);
            result.IsValid.Should().BeFalse();
        }

	    [Test]
	    public void Fails_If_Completion_Status_Is_Negative()
	    {
		    var validator = new EarningDetailsSuperficialValidator();
		    EarningDetails.CompletionStatus = -1;
		    var result = validator.Validate(EarningDetails);
            result.IsValid.Should().BeFalse();
        }

		[Test]
	    public void Fails_If_Monthly_Installment_Is_Negative()
	    {
		    var validator = new EarningDetailsSuperficialValidator();
		    EarningDetails.MonthlyInstallment = -1;
		    var result = validator.Validate(EarningDetails);
            result.IsValid.Should().BeFalse();
        }

	    [Test]
	    public void Fails_If_Total_Installments_Is_Negative()
	    {
		    var validator = new EarningDetailsSuperficialValidator();
		    EarningDetails.TotalInstallments = -1;
		    var result = validator.Validate(EarningDetails);
            result.IsValid.Should().BeFalse();
        }

	    [Test]
	    public void Fails_If_Endpoint_Assesor_Id_Is_Invalid()
	    {
		    var validator = new EarningDetailsSuperficialValidator();
		    EarningDetails.EndpointAssessorId = null;
		    var result = validator.Validate(EarningDetails);
            result.IsValid.Should().BeFalse();
        }
    }
}