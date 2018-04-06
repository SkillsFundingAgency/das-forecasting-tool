using System;
using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class EstimationPageViewModel
    {
        public bool  CanFund { get; set; }
        public IEnumerable<EstimationTransferAllowanceVewModel> TransferAllowances { get; set; }
        public EstimationApprenticeshipsViewModel Apprenticeships { get; set; }
    }
}