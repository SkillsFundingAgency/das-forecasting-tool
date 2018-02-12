using System;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Domain.Commitments.Validation
{
    public class CommitmentEndDateValidator: ICommitmentValidator
    {
        public string Validate(Commitment commitment)
        {
            return !commitment.ActualEndDate.IsNullDate() ? "Commitment has ended" : null;
        }
    }
}