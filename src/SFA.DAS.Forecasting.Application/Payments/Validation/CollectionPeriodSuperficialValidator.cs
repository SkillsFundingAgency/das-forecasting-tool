using SFA.DAS.Forecasting.Application.Payments.Messages;
using FluentValidation;

namespace SFA.DAS.Forecasting.Application.Payments.Validation
{
    public class CollectionPeriodSuperficialValidator : AbstractValidator<NamedCalendarPeriod>
    {
        public CollectionPeriodSuperficialValidator()
        {
            RuleFor(m => m.Id).NotNull().NotEmpty();

            RuleFor(m => m.Month).NotNull().NotEmpty()
                .InclusiveBetween(0 ,12);

            RuleFor(m => m.Year).NotNull().NotEmpty()
                .GreaterThan(0);
        }
    }
}