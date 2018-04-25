using SFA.DAS.Forecasting.Domain.Shared.Validation;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.Estimations.Validation.AddApprenticeshipValidation
{
    public class TotalCapEntered : IAddApprenticeshipValidation
    {
        public ValidationResult Validate(ApprenticeshipToAdd apprenticeshipToAdd)
        {
            return !apprenticeshipToAdd.TotalCost.HasValue || apprenticeshipToAdd.TotalCost <= 0
                ? ValidationResult.Failed("NoCost")
                : ValidationResult.Success;
        }
    }
}