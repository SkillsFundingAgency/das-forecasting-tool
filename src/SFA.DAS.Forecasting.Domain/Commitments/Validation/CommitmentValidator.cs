using FluentValidation;
using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Domain.Commitments.Validation
{
    public class CommitmentValidator : AbstractValidator<Commitment>
    {
        public CommitmentValidator()
        {
            RuleFor(m => m.ActualEndDate).NotNull();
        }
    }
}