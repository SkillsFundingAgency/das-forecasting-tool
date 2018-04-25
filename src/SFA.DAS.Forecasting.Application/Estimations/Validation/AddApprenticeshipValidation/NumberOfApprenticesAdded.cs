using SFA.DAS.Forecasting.Domain.Shared.Validation;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.Estimations.Validation.AddApprenticeshipValidation
{
    public class NumberOfApprenticesAdded : IAddApprenticeshipValidation
    {
        public ValidationResult Validate(ApprenticeshipToAdd apprenticeshipToAdd)
        {
            return apprenticeshipToAdd.ApprenticesCount == null || apprenticeshipToAdd.ApprenticesCount <= 0
                ? ValidationResult.Failed("NoNumberOfApprentices")
                : ValidationResult.Success;
        }
    }
}