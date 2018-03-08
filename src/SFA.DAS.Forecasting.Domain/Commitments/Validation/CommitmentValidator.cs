using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Domain.Commitments.Validation
{
    public interface ICommitmentValidator
    {
        bool IsValid(Commitment commitment);
    }
    public class CommitmentValidator : ICommitmentValidator
    {
        public bool IsValid(Commitment commitment)
        {
            //TODO:  Find out if completion payment has ActualEndDate populated.
            return true;
        }
    }
}