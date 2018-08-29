using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Forecasting.Web.ViewModels;
using SFA.DAS.Forecasting.Web.ViewModels.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Web.UnitTests.Validation
{
    [TestFixture]
    public class AddApprenticeshipViewModelValidatorTests
    {
        private AddApprenticeshipViewModel _validViewModel;
        private AddApprenticeshipViewModelValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validViewModel = new AddApprenticeshipViewModel
            {
                NumberOfApprentices = 2,
                TotalInstallments = 24,
                StartDateMonth = DateTime.Today.Month,
                StartDateYear = DateTime.Today.Year,
                TotalCostAsString = "12000",
                IsTransferFunded = "",
                Course = new Models.Estimation.ApprenticeshipCourse
                {
                    CourseType = Models.Estimation.ApprenticeshipCourseType.Standard,
                    Duration = 12,
                    FundingCap = 2,
                    Id = "hello",
                    Level = 2,
                    Title = "Test Standard",
                    FundingPeriods = new List<Models.Estimation.FundingPeriod>
                    {
                        new Models.Estimation.FundingPeriod
                        {
                            EffectiveFrom = DateTime.Today.AddMonths(-1),
                            EffectiveTo = DateTime.Today.AddMonths(11),
                            FundingCap = 6000
                        }
                    }
                }
        };
            _validator = new AddApprenticeshipViewModelValidator();
        }

        [Test]
        public void ViewModel_Is_Valid()
        {
            var result = _validator.Validate(_validViewModel);
            result.IsValid.Should().BeTrue();
        }

        [TestCase("0")]
        [TestCase("-10")]
        public void Then_The_Total_Cost_Value_Is_Greater_Than_Zero(string totalCost)
        {
            //Arrange
            _validViewModel.TotalCostAsString = totalCost;

            //Act
            var actual = _validator.Validate(_validViewModel);

            //Assert
            var actualError = actual.Errors.SingleOrDefault(m => m.PropertyName == nameof(_validViewModel.TotalCostAsString));
            Assert.IsNotNull(actualError);
            Assert.AreEqual("You must enter a number that is above zero", actualError.ErrorMessage);
        }

        [Test]
        public void ViewModel_must_have_number_of_apprenticeships()
        {
            _validViewModel.NumberOfApprentices = 0;
            var result = _validator.Validate(_validViewModel);
            var error = result.Errors.Single(m => m.PropertyName == nameof(_validViewModel.NumberOfApprentices));
            error.ErrorMessage.Should().Be("Make sure you have at least 1 or more apprentices");
        }

        [Test]
        public void ViewModel_must_have_total_intallments_of_at_least_12s()
        {
            _validViewModel.TotalInstallments = 0;
            var result = _validator.Validate(_validViewModel);
            var error = result.Errors.Single(m => 
                m.PropertyName == nameof(_validViewModel.TotalInstallments)
                &&
                m.ErrorMessage == "The number of months must be between 12 months and 60 months"
                );
            error.ErrorMessage.Should().Be("The number of months must be between 12 months and 60 months");
        }

        [Test]
        public void ViewModel_must_have_start_month()
        {
            _validViewModel.StartDateMonth = 0;
            var result = _validator.Validate(_validViewModel);
            var error = result.Errors.First(m => m.PropertyName == nameof(_validViewModel.StartDateMonth));
            error.ErrorMessage.Should().Be("The start month was not entered");
        }

        [Test]
        public void ViewModel_must_have_start_month_less_that_12()
        {
            _validViewModel.StartDateMonth = 13;
            var result = _validator.Validate(_validViewModel);
            var error = result.Errors.Single(m => m.PropertyName == nameof(_validViewModel.StartDateMonth));
            error.ErrorMessage.Should().Be("The start month entered needs to be between 1 and 12");
        }

        [Test]
        public void ViewModel_must_have_start_year()
        {
            _validViewModel.StartDateYear = 0;
            var result = _validator.Validate(_validViewModel);
            var error = result.Errors.Single(m => m.PropertyName == nameof(_validViewModel.StartDateYear));
            error.ErrorMessage.Should().Be("The start year was not entered");
        }
		
        [Test]
        public void ViewModel_Can_Have_Framework_If_Transfer_Funding()
        {
            _validViewModel.IsTransferFunded = "on";

            var result = _validator.ValidateAdd(_validViewModel);
            result.Count().Should().Be(0);

        }

        [Test]
        public void ViewModel_Cant_Have_Framework_If_Transfer_Funding()
        {
            _validViewModel.IsTransferFunded = "on";
            _validViewModel.Course.Title = "test Framework";
            _validViewModel.Course.CourseType = Models.Estimation.ApprenticeshipCourseType.Framework;

            var result = _validator.ValidateAdd(_validViewModel);

            var error = result.Single(m => m.Key == nameof(_validViewModel.IsTransferFunded));
            error.Value.Should().Be("You can only fund Standards with your transfer allowance");

        }
    }
}
