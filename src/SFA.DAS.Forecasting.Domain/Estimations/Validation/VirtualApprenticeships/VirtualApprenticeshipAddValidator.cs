using System;
using SFA.DAS.Forecasting.Domain.Shared.Validation;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Domain.Estimations.Validation.VirtualApprenticeships
{
    public class VirtualApprenticeshipAddValidator: IVirtualApprenticeshipAddValidator
    {
        public AddApprenticeshipValidationDetail GetCleanValidationDetail()
        {

            return new AddApprenticeshipValidationDetail
            {
                IsValid = true,
                NoCost = "hidden",
                NoNumberOfMonths = "hidden",
                NoApprenticeshipSelected = "hidden",
                NoNumberOfApprentices = "hidden",
                OverCap = "hidden",
                ShortNumberOfMonths = "hidden",
                ValidationSummary = "hidden",
                WrongDate = "hidden"
            };
        }

        public AddApprenticeshipValidationDetail ValidateDetails(ApprenticeshipToAdd apprenticeshipToAdd)
        {
            var validationDetails = GetCleanValidationDetail();
            if (apprenticeshipToAdd.CourseId == null)
            {
                validationDetails.NoApprenticeshipSelected = string.Empty;
                validationDetails.IsValid = false;
            }

            if (apprenticeshipToAdd.ApprenticesCount == null || apprenticeshipToAdd.ApprenticesCount<=0)
            {
                validationDetails.NoNumberOfApprentices = string.Empty;
                validationDetails.IsValid = false;
            }

            if (apprenticeshipToAdd.NumberOfMonths == null || apprenticeshipToAdd.NumberOfMonths <= 0)
            {
                validationDetails.NoNumberOfMonths = string.Empty;
                validationDetails.IsValid = false;
            }

            if (apprenticeshipToAdd.NumberOfMonths.HasValue && apprenticeshipToAdd.NumberOfMonths < 12)
            {
                validationDetails.ShortNumberOfMonths = string.Empty;
                validationDetails.IsValid = false;
            }

            if (!IsValidDate(apprenticeshipToAdd) )
            {
                validationDetails.WrongDate = string.Empty;
                validationDetails.IsValid = false;
            }




            if (!validationDetails.IsValid)
            {
                validationDetails.ValidationSummary = string.Empty;
            }


            return validationDetails;
        }

        private bool IsValidDate(ApprenticeshipToAdd apprenticeshipToAdd)
        {
            if (!apprenticeshipToAdd.StartYear.HasValue || apprenticeshipToAdd.StartMonth.HasValue)
            {
                return false;
            }

            var startYear = apprenticeshipToAdd.StartYear;
            if (startYear < 2000)
            {
                startYear = startYear + 2000;
            }
            
            var dateEntered = new DateTime((int)startYear, (int)apprenticeshipToAdd.StartMonth,1,0,0,0);
            var minDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
            var maxDate = minDate.AddYears(4);

            return dateEntered >= minDate && dateEntered < maxDate;
        }
    }

    public interface IVirtualApprenticeshipAddValidator
    {
        AddApprenticeshipValidationDetail GetCleanValidationDetail();

        AddApprenticeshipValidationDetail ValidateDetails(ApprenticeshipToAdd apprenticeshipToAdd);

    }
}
