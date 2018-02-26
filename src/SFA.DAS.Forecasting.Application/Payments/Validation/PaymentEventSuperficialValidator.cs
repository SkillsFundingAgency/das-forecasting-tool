using SFA.DAS.Forecasting.Application.Payments.Messages;
using FluentValidation;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Application.Payments.Validation
{
    public class PaymentEventSuperficialValidator: AbstractValidator<PaymentCreatedMessage>
    {
        public PaymentEventSuperficialValidator()
        {
            RuleFor(m => m.EmployerAccountId)
                .NotNull().NotEmpty()
                .GreaterThan(0);
	        RuleFor(m => m.Ukprn).GreaterThan(0);
	        RuleFor(m => m.ProviderName).NotNull().NotEmpty();
	        RuleFor(m => m.ApprenticeshipId).GreaterThan(0);
	        RuleFor(m => m.ApprenticeName).NotNull().NotEmpty();
			RuleFor(m => m.Amount).GreaterThan(0);

	        RuleFor(m => m.CourseName).NotNull().NotEmpty();
			
			RuleFor(m => m.EarningDetails)
                .NotNull()
                .SetValidator(new EarningDetailsSuperficialValidator());

            RuleFor(m => m.CollectionPeriod)
                .NotNull()
                .SetValidator(new CollectionPeriodSuperficialValidator());
	        RuleFor(m => m.FundingSource).Must(v => v.HasFlag(FundingSource.Levy) || v.HasFlag(FundingSource.Transfer));
		}
	}
}