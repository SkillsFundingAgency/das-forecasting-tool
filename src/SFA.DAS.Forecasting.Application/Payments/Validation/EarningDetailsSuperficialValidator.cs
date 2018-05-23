using SFA.DAS.Forecasting.Application.Payments.Messages;
using FluentValidation;
using System;

namespace SFA.DAS.Forecasting.Application.Payments.Validation
{
    public class EarningDetailsSuperficialValidator : AbstractValidator<EarningDetails>
    {
        public EarningDetailsSuperficialValidator()
        {
            RuleFor(m => m.StartDate).NotEmpty();
            RuleFor(m => m.PlannedEndDate).NotEmpty();
            RuleFor(m => m.CompletionAmount).GreaterThan(0);
            RuleFor(m => m.MonthlyInstallment).GreaterThan(0);
            RuleFor(m => m.TotalInstallments).GreaterThan(0);

            RuleFor(m => m.MonthlyInstallment + m.CompletionAmount)
                .GreaterThan(3)
                .When(m => m.ActualEndDate == null
                        || m.ActualEndDate == DateTime.MinValue);
        }
    }
}