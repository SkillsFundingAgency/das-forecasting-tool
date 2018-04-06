
namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class EstimationApprenticeshipViewModel
    {
        public string Id { get; set; }
        public string CourseTitle{ get; set; }
        public int Level { get; set; }
        public int Count { get; set; }
        public string StartDate  { get; set; }
        public string MonthlyPayment { get; set; }
        public int MonthlyPaymentCount { get; set; }
        public string ComplementionPayment { get; set; }
    }
}