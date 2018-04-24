using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Forecasting.Domain.Shared.Validation;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.Estimations.Validation
{
    public class AddApprenticeshipValidator : IAddApprenticeshipValidator
    {
        public List<ValidationResult> ValidateApprenticeship(ApprenticeshipToAdd apprenticeshipToAdd)
        {
            var validationResults = new List<ValidationResult>
            {
                ApprenticeshipSelected(apprenticeshipToAdd),
                NumberOfApprenticeshipsEntered(apprenticeshipToAdd),
                NumberOMonthsEntered(apprenticeshipToAdd),
                NumberOfMonthsAcceptable(apprenticeshipToAdd),
                StartMonthEntered(apprenticeshipToAdd),
                StartYearEntered(apprenticeshipToAdd),
                StartDateNotInPast(apprenticeshipToAdd),
                StartDateNotInFarFuture(apprenticeshipToAdd),
                WithinTotalCap(apprenticeshipToAdd),
                TotalCapEntered(apprenticeshipToAdd)
            };

            return validationResults.Where(res => res.IsValid == false).ToList();
        }

        private static ValidationResult ApprenticeshipSelected(ApprenticeshipToAdd apprenticeshipToAdd)
        {
            return apprenticeshipToAdd.CourseId == null
                ? ValidationResult.Failed("NoApprenticeshipSelected")
                : ValidationResult.Success;
        }

        private static ValidationResult NumberOfApprenticeshipsEntered(ApprenticeshipToAdd apprenticeshipToAdd)
        {
            return apprenticeshipToAdd.ApprenticesCount == null || apprenticeshipToAdd.ApprenticesCount <= 0
                ? ValidationResult.Failed("NoNumberOfApprentices")
                : ValidationResult.Success;
        }

        private static ValidationResult NumberOMonthsEntered(ApprenticeshipToAdd apprenticeshipToAdd)
        {
            return apprenticeshipToAdd.NumberOfMonths == null ? ValidationResult.Failed("NoNumberOfMonths") : ValidationResult.Success;
        }

        private static ValidationResult NumberOfMonthsAcceptable(ApprenticeshipToAdd apprenticeshipToAdd)
        {
            return apprenticeshipToAdd.NumberOfMonths.HasValue && apprenticeshipToAdd.NumberOfMonths < 12
                ? ValidationResult.Failed("ShortNumberOfMonths")
                : ValidationResult.Success;
        }

        private static ValidationResult StartMonthEntered(ApprenticeshipToAdd apprenticeshipToAdd)
        {
            return !apprenticeshipToAdd.StartMonth.HasValue || apprenticeshipToAdd.StartMonth.Value < 1 ||
                   apprenticeshipToAdd.StartMonth.Value > 12
                ? ValidationResult.Failed("NoStartMonth")
                : ValidationResult.Success;
        }


        private static ValidationResult StartYearEntered(ApprenticeshipToAdd apprenticeshipToAdd)
        {
            return !apprenticeshipToAdd.StartYear.HasValue
                ? ValidationResult.Failed("NoStartYear")
                : ValidationResult.Success;
        }

        private static ValidationResult StartDateNotInPast(ApprenticeshipToAdd apprenticeshipToAdd)
        {
            if (StartMonthAndStartYearAreAcceptable(apprenticeshipToAdd))
            {
                var startYear = GetStartYearAsFourDigits((int)apprenticeshipToAdd.StartYear);

                var dateEntered = new DateTime(startYear, (int) apprenticeshipToAdd.StartMonth, 1, 0, 0, 0);
                var minDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);

                if (dateEntered < minDate)
                {
                    return ValidationResult.Failed("StartDateInPast");
                }
            }

            return ValidationResult.Success;
        }

        private static ValidationResult StartDateNotInFarFuture(ApprenticeshipToAdd apprenticeshipToAdd)
        {
            if (StartMonthAndStartYearAreAcceptable(apprenticeshipToAdd))
            {
                var startYear = GetStartYearAsFourDigits((int)apprenticeshipToAdd.StartYear);

                var dateEntered = new DateTime(startYear, (int)apprenticeshipToAdd.StartMonth, 1, 0, 0, 0);
                var minDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                var maxDate = minDate.AddYears(4).AddMilliseconds(-1);

                if (dateEntered > maxDate)
                {
                    return ValidationResult.Failed("LateDate");
                }
            }
            return ValidationResult.Success;
        }

        private static int GetStartYearAsFourDigits(int startYear)
        {
             if (startYear < 2000)
            {
                startYear = startYear + 2000;
            }
            return startYear;
        }

        private static ValidationResult WithinTotalCap(ApprenticeshipToAdd apprenticeshipToAdd)
        {
            var fundingCap = apprenticeshipToAdd.AppenticeshipCourse?.FundingCap;
            var noOfApprenticeships = apprenticeshipToAdd.ApprenticesCount;

            if (apprenticeshipToAdd.TotalCost.HasValue
                && fundingCap.HasValue
                && noOfApprenticeships.HasValue && noOfApprenticeships.Value > 0
                && apprenticeshipToAdd.TotalCost > (fundingCap * noOfApprenticeships))
            {
                return ValidationResult.Failed("OverCap");
            }

            return ValidationResult.Success;
        }

        private static ValidationResult TotalCapEntered(ApprenticeshipToAdd apprenticeshipToAdd)
        {
            return !apprenticeshipToAdd.TotalCost.HasValue || apprenticeshipToAdd.TotalCost <= 0
                ? ValidationResult.Failed("NoCost")
                : ValidationResult.Success;
        }

        private static bool StartMonthAndStartYearAreAcceptable(ApprenticeshipToAdd apprenticeshipToAdd)
        {
            return 
                apprenticeshipToAdd.StartMonth.HasValue && 
                apprenticeshipToAdd.StartMonth > 0 && 
                apprenticeshipToAdd.StartMonth < 13 && 
                apprenticeshipToAdd.StartYear.HasValue;
        }
    }

    public interface IAddApprenticeshipValidator
    {
        List<ValidationResult> ValidateApprenticeship(ApprenticeshipToAdd apprenticeshipToAdd);
    }
}