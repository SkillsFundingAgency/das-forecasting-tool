using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Forecasting.Web.Orchestrators.Estimations;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class EstimationPageViewModel
    {
        public string HashedAccountId { get; set; }
        public string EstimationName { get; set; }
        public bool CanFund => TransferAllowances == null ? false : !TransferAllowances.Any(o => o.IsLessThanCost);
        public IEnumerable<EstimationTransferAllowanceVewModel> TransferAllowances { get; set; }
        public EstimationApprenticeshipsViewModel Apprenticeships { get; set; }
        public bool ApprenticeshipRemoved { get; set; }
        public AccountFundsViewModel AccountFunds { get; set; }
    }
}