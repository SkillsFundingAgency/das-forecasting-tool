using System;
using System.Collections.Generic;
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
                .GreaterThan((short)11)
                .WithMessage("The number of months must be 12 months or more");

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
        }

        protected bool CheckTotalCost(AddEditApprenticeshipsViewModel vm, string b)
        {
            var fundingBand = vm.GetFundingPeriod();

            return (b.ToDecimal()) <= (fundingBand.FundingCap * vm.NumberOfApprentices);
        }

        internal Dictionary<string, string> ValidateAdd(AddApprenticeshipViewModel vm)
        {
            var dict = new Dictionary<string, string>();
            if (vm.Course == null)
            {
                dict.Add($"{nameof(vm.Course)}", "You must choose 1 apprenticeship");
            }

            if(vm.TotalCostAsString.ToDecimal() < 1)
                dict.Add($"{nameof(vm.TotalCostAsString)}", "The total training cost was not entered");
            else if(!CheckTotalCost(vm, vm.TotalCostAsString))
                dict.Add($"{nameof(vm.TotalCostAsString)}", "The total cost can't be higher than the total government funding band maximum for this apprenticeship");

            return dict;
        }
    }
}