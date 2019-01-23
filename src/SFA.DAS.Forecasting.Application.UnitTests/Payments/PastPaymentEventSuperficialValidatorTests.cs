using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Converters;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Payments.Validation;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Application.UnitTests.Payments
{
    public class PastPaymentEventSuperficialValidatorTests
    {
        protected PaymentCreatedMessage PaymentCreatedMessage { get; set; }

        [SetUp]
        public void Arrange()
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
                ProviderName = "test provider",
                ApprenticeName = "test apprentice",
                CourseName = "test course",
                CourseLevel = 1,
                Id = Guid.NewGuid().ToString("D"),
                Ukprn = 2,
                FundingSource = FundingSourceConverter.ConvertToApiFundingSource(FundingSource.Levy)
            };
        }

        [Test]
        public void Should_Pass_validation()
        {
            var validator = new PastPaymentEventSuperficialValidator();
            var result = validator.Validate(PaymentCreatedMessage);
            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void Fails_If_Employer_Account_Id_Is_Not_Populated()
        {
            var validator = new PastPaymentEventSuperficialValidator();
            PaymentCreatedMessage.EmployerAccountId = 0;
            var result = validator.Validate(PaymentCreatedMessage);
            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void Fails_If_Ukprn_Is_Negative()
        {
            var validator = new PastPaymentEventSuperficialValidator();
            PaymentCreatedMessage.Ukprn = -1;
            var result = validator.Validate(PaymentCreatedMessage);
            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void Fails_If_Apprenticeship_Id_Is_Negative()
        {
            var validator = new PastPaymentEventSuperficialValidator();
            PaymentCreatedMessage.ApprenticeshipId = -1;
            var result = validator.Validate(PaymentCreatedMessage);
            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void Passes_If_Amount_Is_Negative()
        {
            var validator = new PastPaymentEventSuperficialValidator();
            PaymentCreatedMessage.Amount = -1;
            var result = validator.Validate(PaymentCreatedMessage);
            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void Fails_If_Funding_Source_Is_LessThan_0()
        {
            var validator = new PastPaymentEventSuperficialValidator();
            PaymentCreatedMessage.FundingSource = 0;
            var result = validator.Validate(PaymentCreatedMessage);
            result.IsValid.Should().BeFalse();
        }

        [TestCase(FundingSource.FullyFundedSfa)]
        public void Fails_If_Funding_Source_Is_FullyFundedSfa(FundingSource fundingSource)
        {
            var validator = new PastPaymentEventSuperficialValidator();
            PaymentCreatedMessage.FundingSource = FundingSourceConverter.ConvertToApiFundingSource(fundingSource);
            var result = validator.Validate(PaymentCreatedMessage);
            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void Then_The_Validation_Fails_If_The_FundingSource_Is_Greater_Than_The_Allowed_Values()
        {
            var validator = new PastPaymentEventSuperficialValidator();
            PaymentCreatedMessage.FundingSource = (Provider.Events.Api.Types.FundingSource)6;
            var result = validator.Validate(PaymentCreatedMessage);
            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void Actual_End_date_Is_Something_Message_Is_Still_Validated()
        {
            var validator = new PastPaymentEventSuperficialValidator();
            PaymentCreatedMessage.FundingSource = (Provider.Events.Api.Types.FundingSource)7;
            var result = validator.Validate(PaymentCreatedMessage);
            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void Failes_If_Ids_are_equal_and_FundingSource_Transfer()
        {
            var validator = new PastPaymentEventSuperficialValidator();
            PaymentCreatedMessage.SendingEmployerAccountId = PaymentCreatedMessage.EmployerAccountId;
            PaymentCreatedMessage.FundingSource = FundingSourceConverter.ConvertToApiFundingSource(FundingSource.Transfer);
            var result = validator.Validate(PaymentCreatedMessage);
            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void Fails_If_Ids_are_not_equal_and_FundingSource_Levy()
        {
            var validator = new PastPaymentEventSuperficialValidator();
            PaymentCreatedMessage.SendingEmployerAccountId = PaymentCreatedMessage.EmployerAccountId + 1;
            PaymentCreatedMessage.FundingSource = FundingSourceConverter.ConvertToApiFundingSource(FundingSource.Levy);
            var result = validator.Validate(PaymentCreatedMessage);
            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void Should_have_same_EmployerId_if_Levy()
        {
            var validator = new PastPaymentEventSuperficialValidator();
            PaymentCreatedMessage.SendingEmployerAccountId = PaymentCreatedMessage.EmployerAccountId;
            PaymentCreatedMessage.FundingSource = FundingSourceConverter.ConvertToApiFundingSource(FundingSource.Levy);
            var result = validator.Validate(PaymentCreatedMessage);
            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void Should_have_different_EmployerId_if_transfer()
        {
            var validator = new PastPaymentEventSuperficialValidator();
            PaymentCreatedMessage.SendingEmployerAccountId = PaymentCreatedMessage.EmployerAccountId + 1;
            PaymentCreatedMessage.FundingSource = FundingSourceConverter.ConvertToApiFundingSource(FundingSource.Transfer);
            var result = validator.Validate(PaymentCreatedMessage);
            result.IsValid.Should().BeTrue();
        }
    }
}
