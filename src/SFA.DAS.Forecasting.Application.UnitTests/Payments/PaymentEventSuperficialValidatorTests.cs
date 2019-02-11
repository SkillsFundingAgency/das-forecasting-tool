using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Converters;
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
                SendingEmployerAccountId = 1,
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
                    MonthlyInstallment = 87.27m,
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
                FundingSource = FundingSourceConverter.ConvertToApiFundingSource(FundingSource.Levy)
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
        public void Fails_If_Funding_Source_Is_LessThan_0()
        {
            var validator = new PaymentEventSuperficialValidator();
            PaymentCreatedMessage.FundingSource = 0;
            var result = validator.Validate(PaymentCreatedMessage);
            result.IsValid.Should().BeFalse();
        }

        [TestCase(FundingSource.FullyFundedSfa)]
        public void Fails_If_Funding_Source_Is_CoInvestedEmployer_Or_FullyFundedSfa(FundingSource fundingSource)
        {
            var validator = new PaymentEventSuperficialValidator();
            PaymentCreatedMessage.FundingSource = FundingSourceConverter.ConvertToApiFundingSource(fundingSource);
            var result = validator.Validate(PaymentCreatedMessage);
            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void Then_The_Validation_Fails_If_The_FundingSource_Is_Greater_Than_The_Allowed_Values()
        {
            var validator = new PaymentEventSuperficialValidator();
            PaymentCreatedMessage.FundingSource = (Provider.Events.Api.Types.FundingSource) 6;
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
        public void Actual_End_date_Is_Something_Message_Is_Still_Validated()
        {
            var validator = new PaymentEventSuperficialValidator();
            PaymentCreatedMessage.EarningDetails.ActualEndDate = DateTime.Today;
            PaymentCreatedMessage.FundingSource = (Provider.Events.Api.Types.FundingSource) 7;
            var result = validator.Validate(PaymentCreatedMessage);
            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void Failes_If_Ids_are_equal_and_FundingSource_Transefer()
        {
            var validator = new PaymentEventSuperficialValidator();
            PaymentCreatedMessage.SendingEmployerAccountId = PaymentCreatedMessage.EmployerAccountId;
            PaymentCreatedMessage.FundingSource = FundingSourceConverter.ConvertToApiFundingSource(FundingSource.Transfer);
            var result = validator.Validate(PaymentCreatedMessage);
            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void Fails_If_Ids_are_not_equal_and_FundingSource_Levy()
        {
            var validator = new PaymentEventSuperficialValidator();
            PaymentCreatedMessage.SendingEmployerAccountId = PaymentCreatedMessage.EmployerAccountId + 1;
            PaymentCreatedMessage.FundingSource = FundingSourceConverter.ConvertToApiFundingSource(FundingSource.Levy);
            var result = validator.Validate(PaymentCreatedMessage);
            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void Should_have_same_employerid_if_Levy()
        {
            var validator = new PaymentEventSuperficialValidator();
            PaymentCreatedMessage.SendingEmployerAccountId = PaymentCreatedMessage.EmployerAccountId;
            PaymentCreatedMessage.FundingSource = FundingSourceConverter.ConvertToApiFundingSource(FundingSource.Levy);
            var result = validator.Validate(PaymentCreatedMessage);
            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void Should_have_different_employerid_if_transfer()
        {
            var validator = new PaymentEventSuperficialValidator();
            PaymentCreatedMessage.SendingEmployerAccountId = PaymentCreatedMessage.EmployerAccountId + 1;
            PaymentCreatedMessage.FundingSource = FundingSourceConverter.ConvertToApiFundingSource(FundingSource.Transfer);
            var result = validator.Validate(PaymentCreatedMessage);
            result.IsValid.Should().BeTrue();
        }
    }
}