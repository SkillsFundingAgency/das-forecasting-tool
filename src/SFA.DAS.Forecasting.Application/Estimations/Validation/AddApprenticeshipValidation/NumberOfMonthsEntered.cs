using SFA.DAS.Forecasting.Domain.Shared.Validation;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.Estimations.Validation.AddApprenticeshipValidation
{
    public class NumberOfMonthsEntered : IAddApprenticeshipValidation
    {
        public ValidationResult Validate(ApprenticeshipToAdd apprenticeshipToAdd)
        {
            return apprenticeshipToAdd.NumberOfMonths == null ? ValidationResult.Failed("NoNumberOfMonths") : ValidationResult.Success;
        }
    }
}