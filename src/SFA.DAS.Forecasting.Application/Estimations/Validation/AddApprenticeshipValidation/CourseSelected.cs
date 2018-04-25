using SFA.DAS.Forecasting.Domain.Shared.Validation;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.Estimations.Validation.AddApprenticeshipValidation
{
    public class CourseSelected: IAddApprenticeshipValidation
    {
        public ValidationResult Validate(ApprenticeshipToAdd apprenticeshipToAdd)
        {
            return apprenticeshipToAdd.CourseId == null
                ? ValidationResult.Failed("NoApprenticeshipSelected")
                : ValidationResult.Success;
        }   
    }
}