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
            var deets = GetCleanApprenticeshipToAdd();
            deets.CourseId = null;

            var res = new AddApprenticeshipValidator().ValidateApprenticeship(deets);

            var validationResult = ValidationResult.Failed("NoApprenticeshipSelected");

            res.ShouldBeEquivalentTo(new List<ValidationResult> { validationResult });     
        }

        [TestCase(null)]
        [TestCase(0)]
        public void WhenCallingValidateDetailWithNoApprenticeshipCountShouldReturnNoNumberOfApprentices(int? noOfApprentices)
        {
            var deets = GetCleanApprenticeshipToAdd();
            deets.ApprenticesCount = noOfApprentices;

            var res = new AddApprenticeshipValidator().ValidateApprenticeship(deets);

            var validationResult = ValidationResult.Failed("NoNumberOfApprentices");
            res.ShouldBeEquivalentTo(new List<ValidationResult> { validationResult });
        }

        [Test]
        public void WhenCallingValidateDetailWithNoNumberOfMonthsShouldReturnNoNumberOfMonths()
        {
            var deets = GetCleanApprenticeshipToAdd();
            deets.NumberOfMonths = null;

            var res = new AddApprenticeshipValidator().ValidateApprenticeship(deets);

            var validationResult = ValidationResult.Failed("NoNumberOfMonths");
            res.ShouldBeEquivalentTo(new List<ValidationResult> { validationResult });
        }


        [Test]
        public void WhenCallingValidateDetailWithNoCourseAndNoNumberOfMonthsShouldReturnNoApprenticeshipSelectedAndNoNumberOfMonths()
        {
            var deets = GetCleanApprenticeshipToAdd();
            deets.CourseId = null;
            deets.NumberOfMonths = null;

            var res = new AddApprenticeshipValidator().ValidateApprenticeship(deets);
            
            res.Count.Should().Be(2);
            res.Should().Contain(x => x.FailureReason == "NoApprenticeshipSelected");
            res.Should().Contain(x => x.FailureReason == "NoNumberOfMonths");
        }

        [Test]
        public void WhenCallingASetOfInvalidDetailsShouldReturnSixExpectedValidationResultFailures()
        {
            var deets = new ApprenticeshipToAdd
            {
                CourseId = null,
                ApprenticesCount = null,
                NumberOfMonths = null,
                StartMonth = null,
                StartYear = null,
                TotalCost = null,
                AppenticeshipCourse = null
            };
           
            var res = new AddApprenticeshipValidator().ValidateApprenticeship(deets);

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
            var deets = GetCleanApprenticeshipToAdd();
            deets.NumberOfMonths = 11;

            var res = new AddApprenticeshipValidator().ValidateApprenticeship(deets);

            var validationResult = ValidationResult.Failed("ShortNumberOfMonths");
            res.ShouldBeEquivalentTo(new List<ValidationResult> { validationResult });
        }

        [TestCase(null)]
        [TestCase(0)]
        public void WhenCallingValidateDetailWithInvalidTotalCostShouldReturnNoCost(decimal? totalCost)
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
        public void WhenCallingValidateDetailWithInvalidStartMonthShouldReturnNoStartMonth(int? startMonth)
        {
            var deets = GetCleanApprenticeshipToAdd();
            deets.StartMonth = startMonth;


            var res = new AddApprenticeshipValidator().ValidateApprenticeship(deets);

            var validationResult = ValidationResult.Failed("NoStartMonth");
            res.ShouldBeEquivalentTo(new List<ValidationResult> { validationResult });
        }


        [Test]
        public void WhenCallingValidateDetailWithInvalidStartYearShouldReturnNoStartYear()
        {
            var deets = GetCleanApprenticeshipToAdd();
            deets.StartYear = null;

            var res = new AddApprenticeshipValidator().ValidateApprenticeship(deets);

            var validationResult = ValidationResult.Failed("NoStartYear");
            res.ShouldBeEquivalentTo(new List<ValidationResult> { validationResult });
        }


        [Test]
        public void WhenCallingValidateDetailWithValidStartMonthAndYearShouldReturnNoValidationFailureResult()
        {
            var deets = GetCleanApprenticeshipToAdd();
            var dateNextMonth = DateTime.Now.AddMonths(1);
            deets.StartYear = dateNextMonth.Year;
            deets.StartMonth = dateNextMonth.Month;

            var res = new AddApprenticeshipValidator().ValidateApprenticeship(deets);

              res.ShouldBeEquivalentTo(new List<ValidationResult>());
        }

        [Test]
        public void WhenCallingValidateDetailWithValidStartMonthAndShortYearShouldReturnNoValidationFailureResult()
        {
            var deets = GetCleanApprenticeshipToAdd();
            var dateTakeOff2000 = DateTime.Now.AddYears(-2000).AddMonths(1);
            deets.StartYear = dateTakeOff2000.Year;
            deets.StartMonth = dateTakeOff2000.Month;

            var res = new AddApprenticeshipValidator().ValidateApprenticeship(deets);

            res.ShouldBeEquivalentTo(new List<ValidationResult>());
        }

        [Test]
        public void WhenCallingValidateDetailWithPastStartMonthAndShortYearShouldReturnMatchingStartDateInPastMessage()
        {
            var deets = GetCleanApprenticeshipToAdd();
            deets.StartYear = DateTime.Now.AddMonths(-1).Year;
            deets.StartMonth = DateTime.Now.AddMonths(-1).Month;

            var res = new AddApprenticeshipValidator().ValidateApprenticeship(deets);

            var minDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
            var firstOfMay2018 = new DateTime(2018, 5, 1, 0, 0, 0);
            var validationResult = ValidationResult.Failed(minDate < firstOfMay2018 ? "StartDateBeforeMay2018" : "StartDateInPast");

            res.ShouldBeEquivalentTo(new List<ValidationResult> { validationResult });
        }

        [Test]
        public void WhenCallingValidateDetailWithFarInFutureStartMonthAndShortYearShouldReturnLateDate()
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
        public void WhenCallingValidateDetailWithFundingDetailsShouldReturnOverCapOrSuccessDetails(decimal? totalCost, decimal? fundingCap, int? apprenticeshipsCount, bool isValid)
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

