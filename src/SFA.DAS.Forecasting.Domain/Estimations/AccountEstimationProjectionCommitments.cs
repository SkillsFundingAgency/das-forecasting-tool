using System.Collections.Generic;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Domain.Estimations
{
    public class AccountEstimationProjectionCommitments
    {
        public EmployerCommitments VirtualEmployerCommitments { get; set; }
        public IList<AccountProjectionModel> ActualAccountProjections { get; set; }

        public AccountEstimationProjectionCommitments(EmployerCommitments virtualEmployerCommitments, IList<AccountProjectionModel> actualAccountProjections)
        {
            VirtualEmployerCommitments = virtualEmployerCommitments;
            ActualAccountProjections = actualAccountProjections;
        }
    }
}
