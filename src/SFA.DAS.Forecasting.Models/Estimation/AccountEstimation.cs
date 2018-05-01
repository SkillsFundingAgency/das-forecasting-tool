using SFA.DAS.Forecasting.Models.Projections;
using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Models.Estimation
{
    public class AccountEstimation
    {
        public string  EstimationName { get; set; }
        public long EmployerAccountId { get; set; }

        public IEnumerable<VirtualApprenticeship> Apprenticeships { get; set; }
        public IEnumerable<AccountProjectionReadModel> Estimations { get; set; }

    }
}
