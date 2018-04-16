using SFA.DAS.Forecasting.Domain.Shared.Validation;

namespace SFA.DAS.Forecasting.Domain.Estimations.Validation.VirtualApprenticeships
{
    public class VirtualApprenticeshipCourseDetailsValidator : BaseVirtualApprenticeshipValidator
    {
        public override ValidationResult IsValid(Models.Estimation.VirtualApprenticeship virtualApprenticeship)
        {
            if (string.IsNullOrEmpty(virtualApprenticeship?.CourseId))
                return ValidationResult.Failed("Invalid course id.");

            if (string.IsNullOrEmpty(virtualApprenticeship?.CourseTitle))
                return ValidationResult.Failed("Invalid couse title.");

            if (virtualApprenticeship?.Level<1)
                return ValidationResult.Failed("Invalid course level.");
            //TODO: will probably need to make sure that the course id exists.
            return ValidationResult.Success;
        }
    }
}