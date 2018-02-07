using SFA.DAS.Forecasting.Domain.Commitments.Model;

namespace SFA.DAS.Forecasting.Domain.Commitments.Validation
{
    public interface ICommitmentValidator
    {
        string Validate(Commitment commitment);
    }
}