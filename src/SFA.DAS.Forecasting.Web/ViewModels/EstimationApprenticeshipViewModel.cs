
using System;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class EstimationApprenticeshipViewModel
    {
        public string Id { get; set; }
        public string CourseTitle { get; set; }
        public int Level { get; set; }
        public int ApprenticesCount { get; set; }
        public DateTime StartDate { get; set; }
        public decimal TotalCost { get; set; }
        public decimal MonthlyPayment { get; set; }
        public int MonthlyPaymentCount { get; set; }
        public decimal CompletionPayment { get; set; }
        public FundingSource FundingSource { get; set; }

        public string FundingSourceText
        {
            get
            {
                if (FundingSource == 0 || FundingSource == FundingSource.Transfer)
                    return "Employer transfer";
                if(FundingSource == FundingSource.Levy)
                    return "Account funds";
                return "";
            }
        }
    }
}