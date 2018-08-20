using FluentValidation;
using SFA.DAS.Forecasting.Web.Extensions;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;

namespace SFA.DAS.Forecasting.Web.ViewModels.Validation
{
    public class EditApprenticeshipsViewModelValidator : AddEditApprenticeshipViewModelValidator<EditApprenticeshipsViewModel>
    {
        private readonly IApprenticeshipCourseDataService _courseDataService;

        public EditApprenticeshipsViewModelValidator()
        {
            RuleFor(m => m.TotalCostAsString)
                .Must(s => s.ToDecimal() > 0)
                .WithMessage("The total training cost was not entered")
                .Must((o, b) => CheckTotalCost(o, b))
                .WithMessage("The total cost can't be higher than the total government funding band maximum for this apprenticeship")
                .Must((o, b) => b.ToDecimal() <= o.FundingCapCalculated * o.NumberOfApprentices)
                .WithMessage("The total cost can't be higher than the total government funding band maximum for this apprenticeship")
               ;
        }
    }
}