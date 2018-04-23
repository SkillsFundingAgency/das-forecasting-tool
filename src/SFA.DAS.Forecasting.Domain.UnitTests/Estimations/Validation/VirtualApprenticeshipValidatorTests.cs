using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Estimations.Validation.VirtualApprenticeships;
using SFA.DAS.Forecasting.Domain.Shared.Validation;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Estimations.Validation
{
    [TestFixture]
    public class VirtualApprenticeshipValidatorTests
    {
         private AddApprenticeshipValidationDetail _addApprenticeshipValidationDetail;
         [SetUp]
        public void SetUp()
        {
           _addApprenticeshipValidationDetail = GetCleanValidationDetails();
        }
        
        [Test]
        public void WhenCallingGetCleanValidationDetailShouldReturnCleanDetails()
        {
            var res = new VirtualApprenticeshipAddValidator().GetCleanValidationDetail();

            res.ShouldBeEquivalentTo(_addApprenticeshipValidationDetail);
        }

        [Test]
        public void WhenCallingValidateDetailShouldReturnCleanDetails()
        {
            var res = new VirtualApprenticeshipAddValidator().ValidateDetails(GetCleanApprenticeshipToAdd());

            res.ShouldBeEquivalentTo(_addApprenticeshipValidationDetail);
        }


        [Test]
        public void WhenCallingValidateDetailWithNoCourseShouldReturnExpectedDetails()
        {
            var deets = GetCleanApprenticeshipToAdd();
            deets.CourseId = null;

            var res = new VirtualApprenticeshipAddValidator().ValidateDetails(deets);

            var actualValidation = res;
            var expectedValidation = GetCleanValidationDetails();
            expectedValidation.NoApprenticeshipSelected = string.Empty;
            expectedValidation.IsValid = false;
            expectedValidation.ValidationSummary = string.Empty;

            actualValidation.ShouldBeEquivalentTo(expectedValidation);
        }

        [TestCase(null)]
        [TestCase(0)]
        public void WhenCallingValidateDetailWithNoApprenticeshipCountShouldReturnExpectedDetails(int? noOfApprentices)
        {
            var deets = GetCleanApprenticeshipToAdd();
            deets.ApprenticesCount = noOfApprentices;

            var res = new VirtualApprenticeshipAddValidator().ValidateDetails(deets);

            var actualValidation = res;
            var expectedValidation = GetCleanValidationDetails();
            expectedValidation.NoNumberOfApprentices = string.Empty;
            expectedValidation.IsValid = false;
            expectedValidation.ValidationSummary = string.Empty;

            actualValidation.ShouldBeEquivalentTo(expectedValidation);
        }

        [Test]
        public void WhenCallingValidateDetailWithNoNumberOfMonthsShouldReturnExpectedDetails()
        {
            var deets = GetCleanApprenticeshipToAdd();
            deets.NumberOfMonths = null;

            var res = new VirtualApprenticeshipAddValidator().ValidateDetails(deets);

            var actualValidation = res;
            var expectedValidation = GetCleanValidationDetails();
            expectedValidation.NoNumberOfMonths = string.Empty;
            expectedValidation.IsValid = false;
            expectedValidation.ValidationSummary = string.Empty;

            actualValidation.ShouldBeEquivalentTo(expectedValidation);
        }

        [Test]
        public void WhenCallingValidateDetailWithShortNumberOfMonthsShouldReturnExpectedDetails()
        {
            var deets = GetCleanApprenticeshipToAdd();
            deets.NumberOfMonths = 11;

            var res = new VirtualApprenticeshipAddValidator().ValidateDetails(deets);

            var actualValidation = res;
            var expectedValidation = GetCleanValidationDetails();
            expectedValidation.ShortNumberOfMonths = string.Empty;
            expectedValidation.IsValid = false;
            expectedValidation.ValidationSummary = string.Empty;

            actualValidation.ShouldBeEquivalentTo(expectedValidation);
        }

        [TestCase(null)]
        [TestCase(0)]
        public void WhenCallingValidateDetailWithInvalidTotalCostShouldReturnExpectedDetails(decimal? totalCost)
        {
            var deets = GetCleanApprenticeshipToAdd();
            deets.TotalCost = totalCost;

            var res = new VirtualApprenticeshipAddValidator().ValidateDetails(deets);

            var actualValidation = res;
            var expectedValidation = GetCleanValidationDetails();
            expectedValidation.NoCost = string.Empty;
            expectedValidation.IsValid = false;
            expectedValidation.ValidationSummary = string.Empty;

            actualValidation.ShouldBeEquivalentTo(expectedValidation);
        }

        [TestCase(null)]
        [TestCase(0)]
        [TestCase(13)]
        public void WhenCallingValidateDetailWithInvalidStartMonthShouldReturnExpectedDetails(int? startMonth)
        {
            var deets = GetCleanApprenticeshipToAdd();
            deets.StartMonth = startMonth;

            var res = new VirtualApprenticeshipAddValidator().ValidateDetails(deets);

            var actualValidation = res;
            var expectedValidation = GetCleanValidationDetails();
            expectedValidation.NoStartMonth = string.Empty;
            expectedValidation.IsValid = false;
            expectedValidation.ValidationSummary = string.Empty;

            actualValidation.ShouldBeEquivalentTo(expectedValidation);
        }


        [Test]
        public void WhenCallingValidateDetailWithInvalidStartYearShouldReturnExpectedDetails()
        {
            var deets = GetCleanApprenticeshipToAdd();
            deets.StartYear = null;

            var res = new VirtualApprenticeshipAddValidator().ValidateDetails(deets);

            var actualValidation = res;
            var expectedValidation = GetCleanValidationDetails();
            expectedValidation.NoStartYear = string.Empty;
            expectedValidation.IsValid = false;
            expectedValidation.ValidationSummary = string.Empty;

            actualValidation.ShouldBeEquivalentTo(expectedValidation);
        }


        [Test]
        public void WhenCallingValidateDetailWithValidStartMonthAndYearShouldReturnExpectedDetails()
        {
            var deets = GetCleanApprenticeshipToAdd();
            deets.StartYear = DateTime.Now.Year;
            deets.StartMonth = DateTime.Now.Month;

            var actualValidation = new VirtualApprenticeshipAddValidator().ValidateDetails(deets);
            var expectedValidation = GetCleanValidationDetails();

            actualValidation.ShouldBeEquivalentTo(expectedValidation);
        }

        [Test]
        public void WhenCallingValidateDetailWithValidStartMonthAndShortYearShouldReturnExpectedDetails()
        {
            var deets = GetCleanApprenticeshipToAdd();
            deets.StartYear = DateTime.Now.Year - 2000;
            deets.StartMonth = DateTime.Now.Month;

            var res = new VirtualApprenticeshipAddValidator().ValidateDetails(deets);

            var actualValidation = res;
            var expectedValidation = GetCleanValidationDetails();


            actualValidation.ShouldBeEquivalentTo(expectedValidation);
        }

        [Test]
        public void WhenCallingValidateDetailWithPastStartMonthAndShortYearShouldReturnExpectedDetails()
        {
            var deets = GetCleanApprenticeshipToAdd();
            deets.StartYear = DateTime.Now.AddMonths(-1).Year;
            deets.StartMonth = DateTime.Now.AddMonths(-1).Month;

            var actualValidation = new VirtualApprenticeshipAddValidator().ValidateDetails(deets);
            var expectedValidation = GetCleanValidationDetails();
            expectedValidation.StartDateInPast = string.Empty;
            expectedValidation.IsValid = false;
            expectedValidation.ValidationSummary = string.Empty;

            actualValidation.ShouldBeEquivalentTo(expectedValidation);
        }

        [Test]
        public void WhenCallingValidateDetailWithFarInFutureStartMonthAndShortYearShouldReturnExpectedDetails()
        {
            var deets = GetCleanApprenticeshipToAdd();
            deets.StartYear = DateTime.Now.AddYears(4).Year;
            deets.StartMonth = DateTime.Now.AddYears(4).Month;

            var actualValidation = new VirtualApprenticeshipAddValidator().ValidateDetails(deets);
            var expectedValidation = GetCleanValidationDetails();
            expectedValidation.LateDate = string.Empty;
            expectedValidation.IsValid = false;
            expectedValidation.ValidationSummary = string.Empty;

            actualValidation.ShouldBeEquivalentTo(expectedValidation);
        }

        [Test]
        public void WhenCallingValidateDetailWithOvercapShouldReturnExpectedDetails()
        {
            var deets = GetCleanApprenticeshipToAdd();
            deets.TotalCost = 5000;
            deets.AppenticeshipCourse.FundingCap = 4000;
            deets.ApprenticesCount = 1;


            var actualValidation = new VirtualApprenticeshipAddValidator().ValidateDetails(deets);
            var expectedValidation = GetCleanValidationDetails();
            expectedValidation.OverCap = string.Empty;
            expectedValidation.IsValid = false;
            expectedValidation.ValidationSummary = string.Empty;

            actualValidation.ShouldBeEquivalentTo(expectedValidation);
        }

      
        [TestCase(5000, 4000, 1, false)]
        [TestCase(5000, 4000, 2, true)]
        [TestCase(8000, 4000, 2, true)]
        [TestCase(8000, null, 2, true)]
        public void WhenCallingValidateDetailWithFundingDetailsShouldReturnExpectedDetails(decimal? totalCost, decimal? fundingCap, int? apprenticeshipsCount, bool isValid)
        {

            var classSetting = isValid ? "hidden" : string.Empty;

            var deets = GetCleanApprenticeshipToAdd();
            deets.TotalCost = totalCost;
            if (fundingCap.HasValue)
            {
                deets.AppenticeshipCourse.FundingCap = fundingCap.Value;
            }
            else
            {
                deets.AppenticeshipCourse = null;
            }
            deets.ApprenticesCount = apprenticeshipsCount;


            var actualValidation = new VirtualApprenticeshipAddValidator().ValidateDetails(deets);
            var expectedValidation = GetCleanValidationDetails();
            expectedValidation.OverCap = classSetting;
            expectedValidation.IsValid = isValid;
            expectedValidation.ValidationSummary = classSetting;

            actualValidation.ShouldBeEquivalentTo(expectedValidation);
        }


        private static ApprenticeshipToAdd GetCleanApprenticeshipToAdd()
        {
            return new ApprenticeshipToAdd
            {
                CourseId = "1",
                ApprenticesCount = 1,
                NumberOfMonths = 12,
                StartMonth = 01,
                StartYear = DateTime.Now.AddYears(1).Year,
                TotalCost = 1000,
                AppenticeshipCourse = new ApprenticeshipCourse
                {
                    FundingCap = 1000
                }
            };
        }

        private static AddApprenticeshipValidationDetail GetCleanValidationDetails()
        {
            return new AddApprenticeshipValidationDetail
            {
                IsValid = true,
                NoCost = "hidden",
                NoNumberOfMonths = "hidden",
                NoApprenticeshipSelected = "hidden",
                NoNumberOfApprentices = "hidden",
                NoStartMonth = "hidden",
                NoStartYear = "hidden",
                OverCap = "hidden",
                ShortNumberOfMonths = "hidden",
                ValidationSummary = "hidden",
                LateDate = "hidden",
                StartDateInPast = "hidden"
            };
        }
    }
}

