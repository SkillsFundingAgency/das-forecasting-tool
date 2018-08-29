using System;
using FluentValidation;
using SFA.DAS.Forecasting.Web.Extensions;

namespace SFA.DAS.Forecasting.Web.ViewModels.Validation
{
    public class AddEditApprenticeshipViewModelValidator<T> : AbstractValidator<T> where T : AddEditApprenticeshipsViewModel
    {
        public AddEditApprenticeshipViewModelValidator()
        {
            RuleFor(m => m.NumberOfApprentices)
                .NotNull()
                .WithMessage("Make sure you have at least 1 or more apprentices")
                .GreaterThan(0)
                .WithMessage("Make sure you have at least 1 or more apprentices");

            RuleFor(m => m.TotalInstallments)
                .NotEmpty()
                .WithMessage("The number of months was not entered")
                .InclusiveBetween((short) 12, (short) 60)
                .WithMessage("The number of months must be between 12 months and 60 months");

            RuleFor(m => m.StartDateYear)
                .NotEmpty()
                .WithMessage("The start year was not entered");

            RuleFor(m => m.StartDateMonth)
                .NotEmpty()
                .WithMessage("The start month was not entered")
                .InclusiveBetween(1, 12)
                .WithMessage("The start month entered needs to be between 1 and 12");

            RuleFor(m => m.StartDate)
                .GreaterThan(DateTime.Now.AddMonths(-1))
                .WithMessage("The start date cannot be in the past")
                .When(m => m.StartDate != DateTime.MinValue)
                .LessThanOrEqualTo(DateTime.Now.AddYears(4))
                .WithMessage("The start date must be within the next 4 years")
                .When(m => m.StartDate != DateTime.MinValue);

            RuleFor(m => m.TotalCostAsString)
                .Must(s => s.ToDecimal() > 0)
                .WithMessage("You must enter a number that is above zero");
        }
    }
}