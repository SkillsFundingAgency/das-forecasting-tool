using SFA.DAS.Forecasting.Levy.Application.Messages;
using FluentValidation;

namespace SFA.DAS.Forecasting.Levy.Application.Validation
{
    public class LevyDeclarationEventValidator: AbstractValidator<LevyDeclarationEvent>
    {
        public LevyDeclarationEventValidator()
        {
            RuleFor(e => e.EmployerAccountId).GreaterThan(0);
            RuleFor(e => e.Amount).GreaterThan(0);
            RuleFor(e => e.Scheme).NotNull().NotEmpty();

            RuleFor(e => e.PayrollDate).GreaterThan(System.DateTime.MinValue).NotNull();
            RuleFor(e => e.TransactionDate).GreaterThan(System.DateTime.MinValue).NotNull();
        }
    }
}