
using System;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class EditApprenticeshipsViewModel
    {
        public string ApprenticeshipsId { get; set; }
        public string EstimationName { get; set; }
        public string HashedAccountId { get; set; }
        public string CourseTitle { get; set; }
        public int Level { get; set; }
        public int NumberOfApprentices { get; set; }

        public short TotalInstallments { get; internal set; }
        public decimal TotalCost { get; internal set; }
        public DateTime StartDate { get; internal set; }

        public decimal? CalculatedTotalCap { get; set; }
        public decimal FundingCap { get; internal set; }
    }
}