using SFA.DAS.Forecasting.Application.Estimations.Validation.AddApprenticeshipValidation;
using SFA.DAS.Forecasting.Domain.Shared.Validation;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.Estimations.Validation
{
    public class WithinTotalCap : IAddApprenticeshipValidation
    {
        public ValidationResult Validate(ApprenticeshipToAdd apprenticeshipToAdd)
        {
            var fundingCap = apprenticeshipToAdd.AppenticeshipCourse?.FundingCap;
            var noOfApprenticeships = apprenticeshipToAdd.ApprenticesCount;

            if (apprenticeshipToAdd.TotalCost.HasValue
                && fundingCap.HasValue
                && noOfApprenticeships.HasValue && noOfApprenticeships.Value > 0
                && apprenticeshipToAdd.TotalCost > (fundingCap * noOfApprenticeships))
            {
                return ValidationResult.Failed("OverCap");
            }

            return ValidationResult.Success;
        }
    }
}