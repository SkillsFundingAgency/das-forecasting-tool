using SFA.DAS.Forecasting.Domain.Shared.Validation;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.Estimations.Validation.AddApprenticeshipValidation
{
    public interface IAddApprenticeshipValidation
    {
        ValidationResult Validate(ApprenticeshipToAdd apprenticeshipToAdd);
    }
}