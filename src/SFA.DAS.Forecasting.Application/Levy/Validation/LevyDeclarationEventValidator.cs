using FluentValidation;
using SFA.DAS.Forecasting.Application.Levy.Messages;

namespace SFA.DAS.Forecasting.Levy.Application.Validation
{
    public class LevyDeclarationEventValidator: AbstractValidator<LevyDeclarationEvent>
    {
        public LevyDeclarationEventValidator()
        {
            RuleFor(e => e.EmployerAccountId).GreaterThan(0);
            RuleFor(e => e.Amount).GreaterThan(0);
            RuleFor(e => e.Scheme).NotNull().NotEmpty();

            RuleFor(e => e.PayrollYear).NotNull().NotEmpty();
            RuleFor(e => e.PayrollMonth).NotNull().NotEmpty();
            RuleFor(e => e.TransactionDate).GreaterThan(System.DateTime.MinValue).NotNull();
        }
    }
}