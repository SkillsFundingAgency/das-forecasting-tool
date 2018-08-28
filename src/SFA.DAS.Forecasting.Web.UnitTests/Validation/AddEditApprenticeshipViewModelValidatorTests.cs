using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Forecasting.Web.ViewModels;
using SFA.DAS.Forecasting.Web.ViewModels.Validation;
using System;
using System.Linq;

namespace SFA.DAS.Forecasting.Web.UnitTests.Validation
{
    [TestFixture]
    public class AddEditApprenticeshipViewModelValidatorTests
    {
        private AddEditApprenticeshipsViewModel _validViewModel;
        private AddEditApprenticeshipViewModelValidator<AddEditApprenticeshipsViewModel> _validator;

        [SetUp]
        public void SetUp()
        {
            _validViewModel = new AddEditApprenticeshipsViewModel
            {
                NumberOfApprentices = 2,
                TotalInstallments = 24,
                StartDateMonth = DateTime.Today.Month,
                StartDateYear= DateTime.Today.Year,
                TotalCostAsString = "12000"
            };
            _validator = new AddEditApprenticeshipViewModelValidator<AddEditApprenticeshipsViewModel>();
        }

        [Test]
        public void ViewModel_Is_Valid()
        {
            var result = _validator.Validate(_validViewModel);
            result.IsValid.Should().BeTrue();
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
        public void ViewModel_start_date_must_not_be_in_the_past()
        {
            var date = DateTime.Today.AddMonths(-1);
            _validViewModel.StartDateMonth = date.Month;
            _validViewModel.StartDateYear = date.Year;

            var result = _validator.Validate(_validViewModel);
            var error = result.Errors.Single(m => m.PropertyName == nameof(_validViewModel.StartDate));
            error.ErrorMessage.Should().Be("The start date cannot be in the past");
        }

        [Test]
        public void ViewModel_start_date_must_not_be_more_then_4_years_from_now()
        {
            var date = DateTime.Today.AddYears(4).AddMonths(1);
            _validViewModel.StartDateMonth = date.Month;
            _validViewModel.StartDateYear = date.Year;
            
            var result = _validator.Validate(_validViewModel);
            var error = result.Errors.Single(m => m.PropertyName == nameof(_validViewModel.StartDate));
            error.ErrorMessage.Should().Be("The start date must be within the next 4 years");
        }
    }
}