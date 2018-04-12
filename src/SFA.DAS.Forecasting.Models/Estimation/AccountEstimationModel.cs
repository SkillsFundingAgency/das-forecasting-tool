using SFA.DAS.Forecasting.Models.Projections;
using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Models.Estimation
{
    public class AccountEstimationModel: IDocument
    {
        public string Id { get; set; }
        public string  EstimationName { get; set; }

        public long EmployerAccountId { get; set; }

        public List<VirtualApprenticeship> Apprenticeships { get; set; }

        public AccountEstimationModel()
        {
            Apprenticeships = new List<VirtualApprenticeship>();
        }
    }
}
