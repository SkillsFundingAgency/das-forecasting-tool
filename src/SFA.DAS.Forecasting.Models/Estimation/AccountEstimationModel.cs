using SFA.DAS.Forecasting.Models.Projections;
using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Models.Estimation
{
    public class AccountEstimationModel
    {
        public string Id { get; set; }
        public string  EstimationName { get; set; }
        public long EmployerAccountId { get; set; }

        public List<VirtualApprenticeship> Apprenticeships { get; set; }
    }
}
