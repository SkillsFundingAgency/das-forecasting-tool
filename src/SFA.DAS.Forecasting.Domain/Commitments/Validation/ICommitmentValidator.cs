using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Domain.Commitments.Validation
{
    public interface ICommitmentValidator
    {
        string Validate(Commitment commitment);
    }
}