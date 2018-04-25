using SFA.DAS.Forecasting.Domain.Shared.Validation;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.Estimations.Validation.AddApprenticeshipValidation
{
    public class ValidStartYear : IAddApprenticeshipValidation
    {
        public ValidationResult Validate(ApprenticeshipToAdd apprenticeshipToAdd)
        {
            return !apprenticeshipToAdd.StartYear.HasValue
                ? ValidationResult.Failed("NoStartYear")
                : ValidationResult.Success;
        }
    }
}