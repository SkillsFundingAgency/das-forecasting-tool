using SFA.DAS.Forecasting.Domain.Shared.Validation;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.Estimations.Validation.AddApprenticeshipValidation
{
    public class ValidStartMonth: IAddApprenticeshipValidation
    {
        public ValidationResult Validate(ApprenticeshipToAdd apprenticeshipToAdd)
        {
            return !apprenticeshipToAdd.StartMonth.HasValue || apprenticeshipToAdd.StartMonth.Value < 1 ||
                   apprenticeshipToAdd.StartMonth.Value > 12
                ? ValidationResult.Failed("NoStartMonth")
                : ValidationResult.Success;
        }
    }
}