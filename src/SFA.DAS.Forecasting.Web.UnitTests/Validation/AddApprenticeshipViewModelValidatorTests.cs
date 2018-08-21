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
