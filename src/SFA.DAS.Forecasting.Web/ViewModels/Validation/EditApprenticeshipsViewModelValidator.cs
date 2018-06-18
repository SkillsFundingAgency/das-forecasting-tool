﻿using FluentValidation;
using System;

namespace SFA.DAS.Forecasting.Web.ViewModels.Validation
{
    public class EditApprenticeshipsViewModelValidator : AbstractValidator<EditApprenticeshipsViewModel>
    {
        public EditApprenticeshipsViewModelValidator()
        {
            RuleFor(m => m.NumberOfApprentices)
                .NotNull()
                .WithMessage("Make sure you have at least 1 or more apprentices")
                .GreaterThan(0)
                .WithMessage("Make sure you have at least 1 or more apprentices");


            RuleFor(m => m.TotalInstallments)
                .NotEmpty()
                .WithMessage("The number of months was not entered")
                .GreaterThan((short)0)
                .WithMessage("The number of months must be 12 months or more");


            RuleFor(m => m.StartDateYear)
                .NotEmpty()
                .WithMessage("The start year was not entered");

            RuleFor(m => m.StartDateMonth)
                .NotEmpty()
                .WithMessage("The start month was not entered")
                .InclusiveBetween(1, 12)
                .WithMessage("The start month entered needs to be between 1 and 12");

            RuleFor(m => m.TotalCost)
                .GreaterThan(0)
                .WithMessage("The total training cost was not entered")
                .NotEmpty()
                .WithMessage("The total training cost was not entered");

            RuleFor(m => m.StartDate)
                .GreaterThan(DateTime.Now.AddMonths(-1))
                .WithMessage("The start date cannot be in the past")
                .When(m => m.StartDate != DateTime.MinValue)
                .LessThanOrEqualTo(DateTime.Now.AddYears(4))
                .WithMessage("The start date must be within the next 4 years")
                .When(m => m.StartDate != DateTime.MinValue);

        }
    }
}