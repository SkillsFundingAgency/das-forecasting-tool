using FluentValidation.Attributes;
using SFA.DAS.Forecasting.Web.ViewModels.Validation;
using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Web.ViewModels
{

    [Validator(typeof(EditApprenticeshipsViewModelValidator))]
    public class EditApprenticeshipsViewModel : AddEditApprenticeshipsViewModel
    {
        public EditApprenticeshipsViewModel()
        {
        }

        public string ApprenticeshipsId { get; set; }
        public string EstimationName { get; set; }
        public string HashedAccountId { get; set; }

        public string CourseTitle { get; set; }
        public int Level { get; set; }
        public string CourseId { get; set; }
        public decimal? CalculatedTotalCap => GetFundingPeriod().FundingCap * NumberOfApprentices;
    }
}