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
        public void WhenCallingValidateDetailWithNoCourseShouldReturnNoApprenticeshipSelected()
        {
            var details = GetCleanApprenticeshipToAdd();
            details.CourseId = null;

            var res = new AddApprenticeshipValidator().ValidateApprenticeship(details);

            var validationResult = ValidationResult.Failed("NoApprenticeshipSelected");

            res.ShouldBeEquivalentTo(new List<ValidationResult> { validationResult });     
        }

        [TestCase(null)]
        [TestCase(0)]
        public void WhenCallingValidateDetailWithNoApprenticeshipCountShouldReturnNoNumberOfApprentices(int? noOfApprentices)
        {
            var details = GetCleanApprenticeshipToAdd();
            details.ApprenticesCount = noOfApprentices;

            var res = new AddApprenticeshipValidator().ValidateApprenticeship(details);

            var validationResult = ValidationResult.Failed("NoNumberOfApprentices");
            res.ShouldBeEquivalentTo(new List<ValidationResult> { validationResult });
        }

        [Test]
        public void WhenCallingValidateDetailWithNoNumberOfMonthsShouldReturnNoNumberOfMonths()
        {
            var details = GetCleanApprenticeshipToAdd();
            details.NumberOfMonths = null;

            var res = new AddApprenticeshipValidator().ValidateApprenticeship(details);

            var validationResult = ValidationResult.Failed("NoNumberOfMonths");
            res.ShouldBeEquivalentTo(new List<ValidationResult> { validationResult });
        }


        [Test]
        public void WhenCallingValidateDetailWithNoCourseAndNoNumberOfMonthsShouldReturnNoApprenticeshipSelectedAndNoNumberOfMonths()
        {
            var details = GetCleanApprenticeshipToAdd();
            details.CourseId = null;
            details.NumberOfMonths = null;

            var res = new AddApprenticeshipValidator().ValidateApprenticeship(details);
            
            res.Count.Should().Be(2);
            res.Should().Contain(x => x.FailureReason == "NoApprenticeshipSelected");
            res.Should().Contain(x => x.FailureReason == "NoNumberOfMonths");
        }

        [Test]
        public void WhenCallingASetOfInvalidDetailsShouldReturnSixExpectedValidationResultFailures()
        {
            var details = new ApprenticeshipToAdd
            {
                CourseId = null,
                ApprenticesCount = null,
                NumberOfMonths = null,
                StartMonth = null,
                StartYear = null,
                TotalCost = null,
                AppenticeshipCourse = null
            };
           
            var res = new AddApprenticeshipValidator().ValidateApprenticeship(details);

            res.Count.Should().Be(6);
            res.Should().Contain(x => x.FailureReason == "NoApprenticeshipSelected");
            res.Should().Contain(x => x.FailureReason == "NoNumberOfMonths");
            res.Should().Contain(x => x.FailureReason == "NoNumberOfApprentices");
            res.Should().Contain(x => x.FailureReason == "NoStartMonth");
            res.Should().Contain(x => x.FailureReason == "NoStartYear");
            res.Should().Contain(x => x.FailureReason == "NoCost");
        }


        [Test]
        public void WhenCallingValidateDetailWithShortNumberOfMonthsShouldReturnShortNumberOfMonths()
        {
            var details = GetCleanApprenticeshipToAdd();
            details.NumberOfMonths = 11;

            var res = new AddApprenticeshipValidator().ValidateApprenticeship(details);

            var validationResult = ValidationResult.Failed("ShortNumberOfMonths");
            res.ShouldBeEquivalentTo(new List<ValidationResult> { validationResult });
        }

        [TestCase(null)]
        [TestCase(0)]
        public void WhenCallingValidateDetailWithInvalidTotalCostShouldReturnNoCost(decimal? totalCost)
        {
            var details = GetCleanApprenticeshipToAdd();
            details.TotalCost = totalCost;


            var res = new AddApprenticeshipValidator().ValidateApprenticeship(details);

            var validationResult = ValidationResult.Failed("NoCost");
            res.ShouldBeEquivalentTo(new List<ValidationResult> { validationResult });
        }

        [TestCase(0)]
        [TestCase(13)]
        public void WhenCallingValidateDetailWithInvalidStartMonthShouldReturnInvalidStartMonth(int? startMonth)
        {
            var details = GetCleanApprenticeshipToAdd();
            details.StartMonth = startMonth;


            var res = new AddApprenticeshipValidator().ValidateApprenticeship(details);

            var validationResult = ValidationResult.Failed("InvalidStartMonth");
            res.ShouldBeEquivalentTo(new List<ValidationResult> { validationResult });
        }

        [Test]
        public void WhenCallingValidateDetailWithInvalidStartMonthShouldReturnNoStartMonth()
        {
            var details = GetCleanApprenticeshipToAdd();
            details.StartMonth = null;


            var res = new AddApprenticeshipValidator().ValidateApprenticeship(details);

            var validationResult = ValidationResult.Failed("NoStartMonth");
            res.ShouldBeEquivalentTo(new List<ValidationResult> { validationResult });
        }

        [Test]
        public void WhenCallingValidateDetailWithInvalidStartYearShouldReturnNoStartYear()
        {
            var details = GetCleanApprenticeshipToAdd();
            details.StartYear = null;

            var res = new AddApprenticeshipValidator().ValidateApprenticeship(details);

            var validationResult = ValidationResult.Failed("NoStartYear");
            res.ShouldBeEquivalentTo(new List<ValidationResult> { validationResult });
        }


        [Test]
        public void WhenCallingValidateDetailWithValidStartMonthAndYearShouldReturnNoValidationFailureResult()
        {
            var details = GetCleanApprenticeshipToAdd();
            var dateNextMonth = DateTime.Now.AddMonths(1);
            details.StartYear = dateNextMonth.Year;
            details.StartMonth = dateNextMonth.Month;

            var res = new AddApprenticeshipValidator().ValidateApprenticeship(details);

              res.ShouldBeEquivalentTo(new List<ValidationResult>());
        }

        [Test]
        public void WhenCallingValidateDetailWithValidStartMonthAndShortYearShouldReturnNoValidationFailureResult()
        {
            var details = GetCleanApprenticeshipToAdd();
            var dateTakeOff2000 = DateTime.Now.AddYears(-2000).AddMonths(1);
            details.StartYear = dateTakeOff2000.Year;
            details.StartMonth = dateTakeOff2000.Month;

            var res = new AddApprenticeshipValidator().ValidateApprenticeship(details);

            res.ShouldBeEquivalentTo(new List<ValidationResult>());
        }

        [Test]
        public void WhenCallingValidateDetailWithPastStartMonthAndShortYearShouldReturnMatchingStartDateInPastMessage()
        {
            var details = GetCleanApprenticeshipToAdd();
            details.StartYear = DateTime.Now.AddMonths(-1).Year;
            details.StartMonth = DateTime.Now.AddMonths(-1).Month;

            var res = new AddApprenticeshipValidator().ValidateApprenticeship(details);

            var minDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
            var firstOfMay2018 = new DateTime(2018, 5, 1, 0, 0, 0);
            var validationResult = ValidationResult.Failed(minDate < firstOfMay2018 ? "StartDateBeforeMay2018" : "StartDateInPast");

            res.ShouldBeEquivalentTo(new List<ValidationResult> { validationResult });
        }

        [Test]
        public void WhenCallingValidateDetailWithFarInFutureStartMonthAndShortYearShouldReturnLateDate()
        {
            var details = GetCleanApprenticeshipToAdd();
            details.StartYear = DateTime.Now.AddYears(4).Year;
            details.StartMonth = DateTime.Now.AddYears(4).Month;

            var res = new AddApprenticeshipValidator().ValidateApprenticeship(details);

            var validationResult = ValidationResult.Failed("LateDate");
            res.ShouldBeEquivalentTo(new List<ValidationResult> { validationResult });
        }

        [Test]
        public void WhenCallingValidateDetailWithOvercapShouldReturnOverCap()
        {
            var details = GetCleanApprenticeshipToAdd();
            details.TotalCost = 5000;
            details.AppenticeshipCourse.FundingCap = 4000;
            details.ApprenticesCount = 1;


            var res = new AddApprenticeshipValidator().ValidateApprenticeship(details);

            var validationResult = ValidationResult.Failed("OverCap");
            res.ShouldBeEquivalentTo(new List<ValidationResult> { validationResult });
        }

      
        [TestCase(5000, 4000, 1, false)]
        [TestCase(5000, 4000, 2, true)]
        [TestCase(8000, 4000, 2, true)]
        [TestCase(8000, null, 2, true)]
        public void WhenCallingValidateDetailWithFundingDetailsShouldReturnOverCapOrSuccessDetails(decimal? totalCost, decimal? fundingCap, int? apprenticeshipsCount, bool isValid)
        {

            //var classSetting = isValid ? "hidden" : string.Empty;

            var details = GetCleanApprenticeshipToAdd();
            details.TotalCost = totalCost;
            if (fundingCap.HasValue)
            {
                details.AppenticeshipCourse.FundingCap = fundingCap.Value;
            }
            else
            {
                details.AppenticeshipCourse = null;
            }
            details.ApprenticesCount = apprenticeshipsCount;


            var res = new AddApprenticeshipValidator().ValidateApprenticeship(details);


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

