using System;
using System.Collections.Generic;
using FluentValidation;
using SFA.DAS.Forecasting.Web.Extensions;

namespace SFA.DAS.Forecasting.Web.ViewModels.Validation
{
    public class AddEditApprenticeshipViewModelValidator : AbstractValidator<AddEditApprenticeshipsViewModel>
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
                .InclusiveBetween((short) 12, (short) 100)
                .WithMessage("The number of months must be between 12 months and 100 months");

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

        public Dictionary<string, string> ValidateAdd(AddEditApprenticeshipsViewModel vm)
        {
            var dict = new Dictionary<string, string>();

            if (vm.TotalCostAsString.ToDecimal() <= 0)
            {
                dict.Add($"{nameof(vm.TotalCostAsString)}", "You must enter a number that is above zero");
            }

            if (vm.Course == null)
            {
                dict.Add($"{nameof(vm.Course)}", "You must choose 1 apprenticeship");
            }
            else
            {
                if (vm.IsTransferFunded == "on" && vm.Course.CourseType == Models.Estimation.ApprenticeshipCourseType.Framework)
                    dict.Add($"{nameof(vm.IsTransferFunded)}", "You can only fund Standards with your transfer allowance");
            }

            if ((vm.StartDateYear.ToString().Length < 4) || vm.StartDate < DateTime.Now.AddMonths(-1))
            {
                dict.Add($"{nameof(vm.StartDateYear)}","The start date cannot be in the past");
            }

            return dict;
        }
    }
}