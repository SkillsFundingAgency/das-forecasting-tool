using System;
using FluentValidation.Attributes;
using SFA.DAS.Forecasting.Web.ViewModels.Validation;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    [Validator(typeof(EditApprenticeshipsViewModelValidator))]
    public class EditApprenticeshipsViewModel
    {
        public string ApprenticeshipsId { get; set; }
        public string EstimationName { get; set; }
        public string HashedAccountId { get; set; }
        public string CourseTitle { get; set; }
        public int Level { get; set; }
        public int NumberOfApprentices { get; set; }

        public short TotalInstallments { get; set; }
        public decimal TotalCost { get; set; }

        public DateTime StartDate
        {
            get
            {
                DateTime.TryParse($"{StartDateYear}-{StartDateMonth}-1", out var startDate);
                return startDate;
            }
        }

        public int StartDateMonth { get; set; }
        public int StartDateYear { get; set; }

        public decimal? CalculatedTotalCap { get; set; }
        public decimal FundingCap { get; set; }
    }
}