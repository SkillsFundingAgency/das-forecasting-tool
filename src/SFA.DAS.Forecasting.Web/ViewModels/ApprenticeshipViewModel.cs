using System;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class ApprenticeshipViewModel
    {
        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public decimal MonthlyPayment { get; set; }

        public int TotalInstallments { get; set; }

        public decimal CompletionPayment { get; set; }
    }
}