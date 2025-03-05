using SFA.DAS.Forecasting.Application.Payments.Messages;
using FluentValidation;
using System;

namespace SFA.DAS.Forecasting.Application.Payments.Validation;

public class EarningDetailsSuperficialValidator : AbstractValidator<EarningDetails>
{
    public EarningDetailsSuperficialValidator()
    {
        When(earningDetails => earningDetails.ActualEndDate == DateTime.MinValue, () =>
        {
            RuleFor(m => m.StartDate).NotEmpty();
            RuleFor(m => m.PlannedEndDate).NotEmpty();
            RuleFor(m => m.CompletionAmount).GreaterThan(1);
            RuleFor(m => m.MonthlyInstallment).GreaterThan(1);
            RuleFor(m => m.TotalInstallments).GreaterThan(0);
        });
    }
}