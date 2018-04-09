using SFA.DAS.Forecasting.Models.Projections;
using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Models.Estimation
{
    public class AccountEstimation
    {
        IEnumerable<VirtualApprenticeship> Apprenticeships { get; set; }

        IEnumerable<AccountProjectionReadModel> Estimations { get; set; }

        public int TotalApprenticeshipCount { get; set; }
        public string TotalMonthlyPayment { get; set; }
        public string TotalCompletionPayment { get; set; }

    }
}
