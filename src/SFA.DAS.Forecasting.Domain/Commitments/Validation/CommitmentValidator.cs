using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Domain.Commitments.Validation
{
    public interface ICommitmentValidator
    {
        bool IsValid(CommitmentModel commitment);
    }

    public class CommitmentValidator : ICommitmentValidator
    {
        public bool IsValid(CommitmentModel commitment)
        {
            //TODO:  Find out if completion payment has ActualEndDate populated.
            return true;
        }
    }
}