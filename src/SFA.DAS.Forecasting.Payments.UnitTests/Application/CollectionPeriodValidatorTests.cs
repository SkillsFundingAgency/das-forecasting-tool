using System;
using NUnit.Framework;
using SFA.DAS.Forecasting.Payments.Application.Messages;
using SFA.DAS.Forecasting.Payments.Application.Validation;

namespace SFA.DAS.Forecasting.Payments.UnitTests.Application
{
    [TestFixture]
    public class CollectionPeriodValidatorTests
	{
        protected CollectionPeriod CollectionPeriod { get; set; }

        [SetUp]
        public void SetUp()
        {
	        CollectionPeriod = new CollectionPeriod()
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
			CollectionPeriod.Id = null;
			var result = validator.Validate(CollectionPeriod);
			Assert.IsNotEmpty(result);
		}

		[TestCase("-1")]
		[TestCase("0")]
		[TestCase("13")]
		public void Fails_If_Month_Is_Invalid(string month)
		{
			var validator = new CollectionPeriodSuperficialValidator();
			CollectionPeriod.Month = int.Parse(month);
			var result = validator.Validate(CollectionPeriod);
			Assert.IsNotEmpty(result);
		}

		public void Fails_If_Year_Is_Invalid()
		{
			var validator = new CollectionPeriodSuperficialValidator();
			CollectionPeriod.Month = -1;
			var result = validator.Validate(CollectionPeriod);
			Assert.IsNotEmpty(result);
		}
	}
}