using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Forecasting.Models.Payments;
using SFA.DAS.Forecasting.Web.Orchestrators.Estimations;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class EstimationPageViewModel
    {
        public string HashedAccountId { get; set; }
        public string EstimationName { get; set; }
        public bool CanFund => !TransferAllowances?.Records?.Any(o => o.IsLessThanCost) ?? false;
        public EstimationTransferAllowanceVewModel TransferAllowances { get; set; }
        public EstimationApprenticeshipsViewModel Apprenticeships { get; set; }
        public bool ApprenticeshipRemoved { get; set; }
        public AccountFundsViewModel AccountFunds { get; set; }

        public bool AnyTransferApprenticeships => Any(FundingSource.Transfer);
        public bool AnyLevyApprenticeships => Any(FundingSource.Levy);

        private bool Any(FundingSource fundingSource)
        {
            return 
                Apprenticeships.VirtualApprenticeships.Any(m => (m.FundingSource & fundingSource) == fundingSource);
        }
    }
}