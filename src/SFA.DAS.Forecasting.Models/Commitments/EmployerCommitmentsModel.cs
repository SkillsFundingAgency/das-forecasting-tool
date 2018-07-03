using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Models.Commitments
{
    public class EmployerCommitmentsModel
    {
        public List<CommitmentModel> LevyFundedCommitments { get; set; }
        public List<CommitmentModel> ReceivingEmployerTransferCommitments { get; set; }
        public List<CommitmentModel> SendingEmployerTransferCommitments { get; set; }

        public EmployerCommitmentsModel()
        {
            LevyFundedCommitments = new List<CommitmentModel>();
            ReceivingEmployerTransferCommitments = new List<CommitmentModel>();
            SendingEmployerTransferCommitments = new List<CommitmentModel>();
        }
    }
}
