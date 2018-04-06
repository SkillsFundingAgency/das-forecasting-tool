using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class EstimationApprenticeshipsViewModel
    {
        public IEnumerable<EstimationApprenticeshipViewModel> VirtualApprenticeships { get; set; }
        public int TotalApprenticeshipCount { get; set; }
        public string TotalMonthlyPayment { get; set; }
        public string TotalCompletionPayment { get; set; }
    }
}