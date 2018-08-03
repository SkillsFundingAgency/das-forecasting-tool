using FluentValidation;
using SFA.DAS.Forecasting.Application.Levy.Messages;

namespace SFA.DAS.Forecasting.Application.Levy.Validation
{
    public class LevyDeclarationEventValidator: AbstractValidator<LevySchemeDeclarationUpdatedMessage>
    {
        public LevyDeclarationEventValidator()
        {
            RuleFor(e => e.AccountId).GreaterThan(0);
            RuleFor(e => e.LevyDeclaredInMonth).GreaterThanOrEqualTo(0);
            RuleFor(e => e.EmpRef).NotNull().NotEmpty();

            RuleFor(e => e.PayrollYear).NotNull().NotEmpty();
            RuleFor(e => e.PayrollMonth).NotNull().NotEmpty();
            RuleFor(e => e.CreatedDate).GreaterThan(System.DateTime.MinValue).NotNull();
        }
    }
}