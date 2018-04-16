using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Forecasting.Domain.Shared.Validation;

namespace SFA.DAS.Forecasting.Domain.Estimations.Validation.VirtualApprenticeships
{
    public interface IVirtualApprenticeshipValidator
    {
        List<ValidationResult> Validate(Models.Estimation.VirtualApprenticeship virtualApprenticeship);
    }

    public class VirtualApprenticeshipValidator: IVirtualApprenticeshipValidator
    {
        private readonly List<BaseVirtualApprenticeshipValidator> _validators = new List<BaseVirtualApprenticeshipValidator>
        {
            new VirtualApprenticeshipCourseDetailsValidator()
        };

        public List<ValidationResult> Validate(Models.Estimation.VirtualApprenticeship virtualApprenticeship)
        {
            return _validators.Select(validator => validator.IsValid(virtualApprenticeship)).ToList();
        }
    }
}