using SFA.DAS.Forecasting.Domain.Shared.Validation;

namespace SFA.DAS.Forecasting.Domain.Estimations.Validation.VirtualApprenticeships
{
    public abstract  class BaseVirtualApprenticeshipValidator
    {
        public abstract ValidationResult IsValid(Models.Estimation.VirtualApprenticeship virtualApprenticeship);
    }
}