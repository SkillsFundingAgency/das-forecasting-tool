using System;
using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class EstimationPageViewModel
    {
        public IEnumerable<EstimationTransferAllowanceVewModel> TransferAllowances { get; set; }
        public IEnumerable<EstimationApprenticeshipViewModel> Apprenticeships { get; set; }
    }
}