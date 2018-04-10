using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class EstimationApprenticeshipsViewModel
    {
        public IEnumerable<EstimationApprenticeshipViewModel> VirtualApprenticeships { get; set; }
        public int TotalApprenticeshipCount => VirtualApprenticeships?.Sum(o => o.ApprenticesCount) ?? 0;
        public decimal TotalMonthlyPayment => VirtualApprenticeships?.Sum(o => o.MonthlyPayment) ?? 0;
        public decimal TotalCompletionPayment => VirtualApprenticeships?.Sum(o => o.CompletionPayment) ?? 0;
    }
}