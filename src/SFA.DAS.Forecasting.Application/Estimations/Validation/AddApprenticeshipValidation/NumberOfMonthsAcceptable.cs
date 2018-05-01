using SFA.DAS.Forecasting.Domain.Shared.Validation;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.Estimations.Validation.AddApprenticeshipValidation
{
    public class NumberOfMonthsAcceptable : IAddApprenticeshipValidation
    {
        public ValidationResult Validate(ApprenticeshipToAdd apprenticeshipToAdd)
        {
            return apprenticeshipToAdd.NumberOfMonths.HasValue && apprenticeshipToAdd.NumberOfMonths < 12
                ? ValidationResult.Failed("ShortNumberOfMonths")
                : ValidationResult.Success;
        }
    }
}