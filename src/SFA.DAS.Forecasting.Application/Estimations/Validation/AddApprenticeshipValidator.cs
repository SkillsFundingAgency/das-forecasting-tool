using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Forecasting.Application.Estimations.Validation.AddApprenticeshipValidation;
using SFA.DAS.Forecasting.Domain.Shared.Validation;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.Estimations.Validation
{
    public class AddApprenticeshipValidator : IAddApprenticeshipValidator
    {
        private readonly CourseSelected _courseSelected = new CourseSelected();
        private readonly NumberOfApprenticesAdded _numberOfApprenticesAdded = new NumberOfApprenticesAdded();
        private readonly NumberOfMonthsEntered _numberOfMonthsEntered = new NumberOfMonthsEntered();
        private readonly NumberOfMonthsAcceptable _numberOfMonthsAcceptable = new NumberOfMonthsAcceptable();
        private readonly ValidStartMonth _validStartMonth = new ValidStartMonth();
        private readonly ValidStartYear _validStartYear = new ValidStartYear();
        private readonly ValidStartDate _validStartDate = new ValidStartDate();
        private readonly WithinTotalCap _withinTotalCap = new WithinTotalCap();
        private readonly TotalCapEntered _totalCapEntered = new TotalCapEntered();


        public List<ValidationResult> ValidateApprenticeship(ApprenticeshipToAdd apprenticeshipToAdd)
        {
            var validationResults = new List<ValidationResult>
            {
                _courseSelected.Validate(apprenticeshipToAdd),
                _numberOfApprenticesAdded.Validate(apprenticeshipToAdd),
                _numberOfMonthsEntered.Validate(apprenticeshipToAdd),
                _numberOfMonthsAcceptable.Validate(apprenticeshipToAdd),
                _validStartMonth.Validate(apprenticeshipToAdd),
                _validStartYear.Validate(apprenticeshipToAdd),
                _validStartDate.Validate(apprenticeshipToAdd),
                _withinTotalCap.Validate(apprenticeshipToAdd),
                _totalCapEntered.Validate(apprenticeshipToAdd)
            };

            return validationResults.Where(res => res.IsValid == false).ToList();
        }
    }

    public interface IAddApprenticeshipValidator
    {
        List<ValidationResult> ValidateApprenticeship(ApprenticeshipToAdd apprenticeshipToAdd);
    }
}