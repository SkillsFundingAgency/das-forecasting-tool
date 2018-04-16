
using System;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class EstimationApprenticeshipViewModel
    {
        public string Id { get; set; }
        public string CourseTitle{ get; set; }
        public int Level { get; set; }
        public int ApprenticesCount { get; set; }
        public DateTime StartDate  { get; set; }
        public decimal MonthlyPayment { get; set; }
        public int MonthlyPaymentCount { get; set; }
        public decimal CompletionPayment { get; set; }
    }
}