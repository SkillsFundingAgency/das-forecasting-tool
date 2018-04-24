using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Estimations.Validation;
using SFA.DAS.Forecasting.Domain.Shared.Validation;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.UnitTests.Estimations.Validation
{
    [TestFixture]
    public class AddApprenticeshipValidatorTests
    {  
        
        [Test]
        public void WhenCallingCleanDataShouldReturnNoValidationFailures()
        {
            var apprenticeship = GetCleanApprenticeshipToAdd();
            var res = new AddApprenticeshipValidator().ValidateApprenticeship(apprenticeship);

            res.ShouldBeEquivalentTo(new List<ValidationResult>());
        }

        
        [Test]
        public void WhenCallingValidateDetailWithNoCourseShouldReturnExpectedDetails()
        {
            var deets = GetCleanApprenticeshipToAdd();
            deets.CourseId = null;

            var res = new AddApprenticeshipValidator().ValidateApprenticeship(deets);

            var validationResult = ValidationResult.Failed("NoApprenticeshipSelected");

            res.ShouldBeEquivalentTo(new List<ValidationResult> { validationResult });     
        }

        [TestCase(null)]
        [TestCase(0)]
        public void WhenCallingValidateDetailWithNoApprenticeshipCountShouldReturnExpectedDetails(int? noOfApprentices)
        {
            var deets = GetCleanApprenticeshipToAdd();
            deets.ApprenticesCount = noOfApprentices;

            var res = new AddApprenticeshipValidator().ValidateApprenticeship(deets);

            var validationResult = ValidationResult.Failed("NoNumberOfApprentices");
            res.ShouldBeEquivalentTo(new List<ValidationResult> { validationResult });
        }

        [Test]
        public void WhenCallingValidateDetailWithNoNumberOfMonthsShouldReturnExpectedDetails()
        {
            var deets = GetCleanApprenticeshipToAdd();
            deets.NumberOfMonths = null;

            var res = new AddApprenticeshipValidator().ValidateApprenticeship(deets);

            var validationResult = ValidationResult.Failed("NoNumberOfMonths");
            res.ShouldBeEquivalentTo(new List<ValidationResult> { validationResult });
        }

        [Test]
        public void WhenCallingValidateDetailWithShortNumberOfMonthsShouldReturnExpectedDetails()
        {
            var deets = GetCleanApprenticeshipToAdd();
            deets.NumberOfMonths = 11;

            var res = new AddApprenticeshipValidator().ValidateApprenticeship(deets);

            var validationResult = ValidationResult.Failed("ShortNumberOfMonths");
            res.ShouldBeEquivalentTo(new List<ValidationResult> { validationResult });
        }

        [TestCase(null)]
        [TestCase(0)]
        public void WhenCallingValidateDetailWithInvalidTotalCostShouldReturnExpectedDetails(decimal? totalCost)
        {
            var deets = GetCleanApprenticeshipToAdd();
            deets.TotalCost = totalCost;


            var res = new AddApprenticeshipValidator().ValidateApprenticeship(deets);

            var validationResult = ValidationResult.Failed("NoCost");
            res.ShouldBeEquivalentTo(new List<ValidationResult> { validationResult });
        }

        [TestCase(null)]
        [TestCase(0)]
        [TestCase(13)]
        public void WhenCallingValidateDetailWithInvalidStartMonthShouldReturnExpectedDetails(int? startMonth)
        {
            var deets = GetCleanApprenticeshipToAdd();
            deets.StartMonth = startMonth;


            var res = new AddApprenticeshipValidator().ValidateApprenticeship(deets);

            var validationResult = ValidationResult.Failed("NoStartMonth");
            res.ShouldBeEquivalentTo(new List<ValidationResult> { validationResult });
        }


        [Test]
        public void WhenCallingValidateDetailWithInvalidStartYearShouldReturnExpectedDetails()
        {
            var deets = GetCleanApprenticeshipToAdd();
            deets.StartYear = null;

            var res = new AddApprenticeshipValidator().ValidateApprenticeship(deets);

            var validationResult = ValidationResult.Failed("NoStartYear");
            res.ShouldBeEquivalentTo(new List<ValidationResult> { validationResult });
        }


        [Test]
        public void WhenCallingValidateDetailWithValidStartMonthAndYearShouldReturnExpectedDetails()
        {
            var deets = GetCleanApprenticeshipToAdd();
            deets.StartYear = DateTime.Now.Year;
            deets.StartMonth = DateTime.Now.Month;

            var res = new AddApprenticeshipValidator().ValidateApprenticeship(deets);

              res.ShouldBeEquivalentTo(new List<ValidationResult>());
        }

        [Test]
        public void WhenCallingValidateDetailWithValidStartMonthAndShortYearShouldReturnExpectedDetails()
        {
            var deets = GetCleanApprenticeshipToAdd();
            deets.StartYear = DateTime.Now.Year - 2000;
            deets.StartMonth = DateTime.Now.Month;

            var res = new AddApprenticeshipValidator().ValidateApprenticeship(deets);

            res.ShouldBeEquivalentTo(new List<ValidationResult>());
        }

        [Test]
        public void WhenCallingValidateDetailWithPastStartMonthAndShortYearShouldReturnExpectedDetails()
        {
            var deets = GetCleanApprenticeshipToAdd();
            deets.StartYear = DateTime.Now.AddMonths(-1).Year;
            deets.StartMonth = DateTime.Now.AddMonths(-1).Month;

            var res = new AddApprenticeshipValidator().ValidateApprenticeship(deets);

            var validationResult = ValidationResult.Failed("StartDateInPast");
            res.ShouldBeEquivalentTo(new List<ValidationResult> { validationResult });
        }

        [Test]
        public void WhenCallingValidateDetailWithFarInFutureStartMonthAndShortYearShouldReturnExpectedDetails()
        {
            var deets = GetCleanApprenticeshipToAdd();
            deets.StartYear = DateTime.Now.AddYears(4).Year;
            deets.StartMonth = DateTime.Now.AddYears(4).Month;

            var res = new AddApprenticeshipValidator().ValidateApprenticeship(deets);

            var validationResult = ValidationResult.Failed("LateDate");
            res.ShouldBeEquivalentTo(new List<ValidationResult> { validationResult });
        }

        [Test]
        public void WhenCallingValidateDetailWithOvercapShouldReturnExpectedDetails()
        {
            var deets = GetCleanApprenticeshipToAdd();
            deets.TotalCost = 5000;
            deets.AppenticeshipCourse.FundingCap = 4000;
            deets.ApprenticesCount = 1;


            var res = new AddApprenticeshipValidator().ValidateApprenticeship(deets);

            var validationResult = ValidationResult.Failed("OverCap");
            res.ShouldBeEquivalentTo(new List<ValidationResult> { validationResult });
        }

      
        [TestCase(5000, 4000, 1, false)]
        [TestCase(5000, 4000, 2, true)]
        [TestCase(8000, 4000, 2, true)]
        [TestCase(8000, null, 2, true)]
        public void WhenCallingValidateDetailWithFundingDetailsShouldReturnExpectedDetails(decimal? totalCost, decimal? fundingCap, int? apprenticeshipsCount, bool isValid)
        {

            //var classSetting = isValid ? "hidden" : string.Empty;

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


            var res = new AddApprenticeshipValidator().ValidateApprenticeship(deets);


            res.ShouldBeEquivalentTo(!isValid
                ? new List<ValidationResult> {ValidationResult.Failed("OverCap")}
                : new List<ValidationResult>());
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
    }
}

