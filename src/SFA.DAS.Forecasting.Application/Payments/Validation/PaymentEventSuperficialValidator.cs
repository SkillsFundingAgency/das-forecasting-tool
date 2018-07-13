using SFA.DAS.Forecasting.Application.Payments.Messages;
using FluentValidation;
using SFA.DAS.Forecasting.Models.Payments;
using System;
using SFA.DAS.Forecasting.Application.Converters;

namespace SFA.DAS.Forecasting.Application.Payments.Validation
{
    public class PaymentEventSuperficialValidator : AbstractValidator<PaymentCreatedMessage>
    {
        public PaymentEventSuperficialValidator()
        {
            RuleFor(m => m.EmployerAccountId)
                .NotNull().NotEmpty()
                .GreaterThan(0);
            RuleFor(m => m.ApprenticeshipId).GreaterThan(0);
            RuleFor(m => m.FundingSource)
				.Must(v => v.Equals(FundingSourceConverter.ConvertToApiFundingSource(FundingSource.Levy)) 
						|| v.Equals(FundingSourceConverter.ConvertToApiFundingSource(FundingSource.Transfer)) 
						|| v.Equals(FundingSourceConverter.ConvertToApiFundingSource(FundingSource.CoInvestedEmployer)) 
						|| v.Equals(FundingSourceConverter.ConvertToApiFundingSource(FundingSource.CoInvestedSfa)));
            When(payment => (payment.EarningDetails?.ActualEndDate ?? DateTime.MinValue) == DateTime.MinValue, () => {
                RuleFor(m => m.Ukprn).GreaterThan(0);
                RuleFor(m => m.ProviderName).NotNull().NotEmpty();
                RuleFor(m => m.ApprenticeName).NotNull().NotEmpty();

                //TODO: Until the Expired Funds story is ready there is no point in validating the payment amount.
                //RuleFor(m => m.Amount).GreaterThan(1);

                RuleFor(m => m.CourseName).NotNull().NotEmpty();

                RuleFor(m => m.SendingEmployerAccountId)
                    .NotEqual(m => m.EmployerAccountId)
                    .When(m => m.FundingSource == FundingSourceConverter.ConvertToApiFundingSource(FundingSource.Transfer))
                    .WithMessage(m => $"{nameof(m.SendingEmployerAccountId)} and {nameof(m.EmployerAccountId)} must not be equal if FundingSource is {FundingSource.Transfer}");

                RuleFor(m => m.SendingEmployerAccountId)
                    .Equal(m => m.EmployerAccountId)
                    .When(m => m.FundingSource == FundingSourceConverter.ConvertToApiFundingSource(FundingSource.Levy))
                    .WithMessage(m => $"{nameof(m.SendingEmployerAccountId)} and {nameof(m.EmployerAccountId)} must be equal if FundingSource is {FundingSource.Levy}");

                RuleFor(m => m.SendingEmployerAccountId)
                        .NotEqual(m => m.EmployerAccountId)
                        .When(m => m.FundingSource == FundingSourceConverter.ConvertToApiFundingSource(FundingSource.Transfer))
                        .WithMessage(m => $"{nameof(m.SendingEmployerAccountId)} and {nameof(m.EmployerAccountId)} must not be equal if FundingSource is {FundingSource.Transfer}");

                RuleFor(m => m.EarningDetails)
                        .NotNull()
                        .SetValidator(new EarningDetailsSuperficialValidator());

                RuleFor(m => m.CollectionPeriod)
                    .NotNull()
                    .SetValidator(new CollectionPeriodSuperficialValidator());

                RuleFor(m => m.FundingSource).Must(v => v.HasFlag(FundingSource.Levy) || v.HasFlag(FundingSource.Transfer));

                RuleFor(m => m.SendingEmployerAccountId)
                    .NotEqual(m => m.EmployerAccountId)
                    .When(m => m.FundingSource == FundingSourceConverter.ConvertToApiFundingSource(FundingSource.Transfer))
                    .WithMessage(m => $"{nameof(m.SendingEmployerAccountId)} and {nameof(m.SendingEmployerAccountId)} must not be equal if FundingSource is {FundingSource.Transfer}");
            });
        }
	}
}