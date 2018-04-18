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
                NoStartMonth = "hidden",
                NoStartYear = "hidden",
                OverCap = "hidden",
                ShortNumberOfMonths = "hidden",
                ValidationSummary = "hidden",
                LateDate = "hidden",
                StartDateInPast = "hidden"
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

            validationDetails = ProcessDatesAndReturnValidity(apprenticeshipToAdd, validationDetails);

            if (!apprenticeshipToAdd.TotalCost.HasValue || apprenticeshipToAdd.TotalCost <= 0)
            {
                validationDetails.NoCost = string.Empty;
                validationDetails.IsValid = false;
            }

            var fundingCap = apprenticeshipToAdd.AppenticeshipCourse?.FundingCap;
            var noOfApprenticeships = apprenticeshipToAdd.ApprenticesCount;

            if (apprenticeshipToAdd.TotalCost.HasValue 
                && fundingCap.HasValue 
                && noOfApprenticeships.HasValue 
                && apprenticeshipToAdd.TotalCost > (fundingCap * noOfApprenticeships))
            {
                validationDetails.OverCap = string.Empty;
                validationDetails.IsValid = false;
            }



            if (!validationDetails.IsValid)
            {
                validationDetails.ValidationSummary = string.Empty;
            }


            return validationDetails;
        }

        private AddApprenticeshipValidationDetail ProcessDatesAndReturnValidity(ApprenticeshipToAdd apprenticeshipToAdd, AddApprenticeshipValidationDetail validationDetails)
        {
            var isValid = true;

            if (!apprenticeshipToAdd.StartMonth.HasValue || apprenticeshipToAdd.StartMonth.Value < 1 || apprenticeshipToAdd.StartMonth.Value > 12)
            {
                validationDetails.NoStartMonth = string.Empty;
                isValid = false;
            }

            if (!apprenticeshipToAdd.StartYear.HasValue)
            {
                validationDetails.NoStartYear = string.Empty;
                isValid = false;
            }

            if (!isValid)
            {
                validationDetails.ValidationSummary = string.Empty;
                validationDetails.IsValid = false;
                return validationDetails;
            }

            var startYear = apprenticeshipToAdd.StartYear;
            if (startYear < 2000)
            {
                startYear = startYear + 2000;
            }
            
            var dateEntered = new DateTime((int)startYear, (int)apprenticeshipToAdd.StartMonth,1,0,0,0);
            var minDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
            var maxDate = minDate.AddYears(4).AddMilliseconds(-1);

            if (dateEntered < minDate)
            {
                validationDetails.StartDateInPast = string.Empty;
                isValid = false;
            }

            if (dateEntered > maxDate)
            {
                validationDetails.LateDate = string.Empty;
                isValid = false;
            }
            
            if (!isValid)
            {
                validationDetails.ValidationSummary = string.Empty;
                validationDetails.IsValid = false;
            }

            return validationDetails;
        }
    }

    public interface IVirtualApprenticeshipAddValidator
    {
        AddApprenticeshipValidationDetail GetCleanValidationDetail();

        AddApprenticeshipValidationDetail ValidateDetails(ApprenticeshipToAdd apprenticeshipToAdd);

    }
}
