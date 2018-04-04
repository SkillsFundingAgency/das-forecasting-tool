using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class EstimationApprenticeshipViewModel
    {
        public string Id { get; set; }
        public string ApprenticeshipSummary{ get; set; }
        public int Count { get; set; }
        public string StartDate  { get; set; }
        public string MonthlyPayment { get; set; }
        public int MonthlyPaymentCount { get; set; }
        public string ComplementionPayment { get; set; }
    }
}