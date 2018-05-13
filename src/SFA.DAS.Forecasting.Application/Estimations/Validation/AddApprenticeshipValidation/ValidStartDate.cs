using System;
using SFA.DAS.Forecasting.Domain.Shared.Validation;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.Estimations.Validation.AddApprenticeshipValidation
{
    public class ValidStartDate : IAddApprenticeshipValidation
    {
        public ValidationResult Validate(ApprenticeshipToAdd apprenticeshipToAdd)
        {
            if (StartMonthAndStartYearAreAcceptable(apprenticeshipToAdd))
            {
                var startYear = GetStartYearAsFourDigits(apprenticeshipToAdd.StartYear.Value);
                var dateEntered = new DateTime(startYear, apprenticeshipToAdd.StartMonth.Value, 1, 0, 0, 0);
                var minDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                var maxDate = minDate.AddYears(4).AddMilliseconds(-1);
                var firstOfMay2018 = new DateTime(2018, 5, 1, 0, 0, 0);

                if (dateEntered < firstOfMay2018 && minDate < firstOfMay2018)
                {
                    return ValidationResult.Failed("StartDateBeforeMay2018");
                }

                if (dateEntered < minDate)
                {
                    return ValidationResult.Failed("StartDateInPast");
                }

                if (dateEntered > maxDate)
                {
                    return ValidationResult.Failed("LateDate");
                }
            }

            return ValidationResult.Success;
        }

        public int GetStartYearAsFourDigits(int startYear)
        {
            if (startYear < 2000)
            {
                startYear = startYear + 2000;
            }
            return startYear;
        }

        public bool StartMonthAndStartYearAreAcceptable(ApprenticeshipToAdd apprenticeshipToAdd)
        {
            return
                apprenticeshipToAdd.StartMonth.HasValue &&
                apprenticeshipToAdd.StartMonth > 0 &&
                apprenticeshipToAdd.StartMonth < 13 &&
                apprenticeshipToAdd.StartYear.HasValue;
        }
    }
}