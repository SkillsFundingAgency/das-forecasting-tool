using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Payments.Validation;

namespace SFA.DAS.Forecasting.Application.UnitTests.Payments
{
    [TestFixture]
    public class CollectionPeriodValidatorTests
	{
        protected NamedCalendarPeriod NamedCalendarPeriod { get; set; }

        [SetUp]
        public void SetUp()
        {
	        NamedCalendarPeriod = new NamedCalendarPeriod()
            {
				Id = Guid.NewGuid().ToString("N"),
                Month = DateTime.Today.Month,
                Year = DateTime.Today.Year
            };
        }

		[Test]
		public void Fails_If_Id_Is_Invalid()
		{
			var validator = new CollectionPeriodSuperficialValidator();
			NamedCalendarPeriod.Id = null;
			var result = validator.Validate(NamedCalendarPeriod);
            result.IsValid.Should().BeFalse();
        }

		[TestCase("-1")]
		[TestCase("0")]
		[TestCase("13")]
		public void Fails_If_Month_Is_Invalid(string month)
		{
			var validator = new CollectionPeriodSuperficialValidator();
			NamedCalendarPeriod.Month = int.Parse(month);
			var result = validator.Validate(NamedCalendarPeriod);
            result.IsValid.Should().BeFalse();
		}

		public void Fails_If_Year_Is_Invalid()
		{
			var validator = new CollectionPeriodSuperficialValidator();
			NamedCalendarPeriod.Month = -1;
			var result = validator.Validate(NamedCalendarPeriod);
            result.IsValid.Should().BeFalse();
        }
	}
}