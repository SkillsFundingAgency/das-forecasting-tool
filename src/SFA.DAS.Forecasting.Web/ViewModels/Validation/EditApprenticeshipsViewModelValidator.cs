using FluentValidation;
using SFA.DAS.Forecasting.Web.Extensions;

namespace SFA.DAS.Forecasting.Web.ViewModels.Validation
{
    public class EditApprenticeshipsViewModelValidator : AddEditApprenticeshipViewModelValidator<EditApprenticeshipsViewModel>
    {
        public EditApprenticeshipsViewModelValidator()
        {
            RuleFor(m => m.TotalCostAsString)
                .Must(s => s.ToDecimal() > 0)
                .WithMessage("You must enter a number that is above zero");
        }
    }
}