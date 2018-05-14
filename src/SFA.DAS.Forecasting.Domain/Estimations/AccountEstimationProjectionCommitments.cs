using SFA.DAS.Forecasting.Domain.Commitments;

namespace SFA.DAS.Forecasting.Domain.Estimations
{
    public class AccountEstimationProjectionCommitments
    {
        public EmployerCommitments VirtualEmployerCommitments { get; set; }
        public EmployerCommitments TransferEmployerCommitments { get; set; }

        public AccountEstimationProjectionCommitments(EmployerCommitments virtualEmployerCommitments, EmployerCommitments transferEmployerCommitments)
        {
            VirtualEmployerCommitments = virtualEmployerCommitments;
            TransferEmployerCommitments = transferEmployerCommitments;
        }
    }
}
