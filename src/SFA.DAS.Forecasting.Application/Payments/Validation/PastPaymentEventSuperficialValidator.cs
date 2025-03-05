using FluentValidation;
using SFA.DAS.Forecasting.Application.Converters;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Application.Payments.Validation;

public class PastPaymentEventSuperficialValidator : AbstractValidator<PaymentCreatedMessage>
{
    public PastPaymentEventSuperficialValidator()
    {
        RuleFor(m => m.EmployerAccountId)
            .NotNull().NotEmpty()
            .GreaterThan(0);
        RuleFor(m => m.ApprenticeshipId).GreaterThan(0);
        RuleFor(m => m.FundingSource)
            .Must(v => v.Equals(FundingSourceConverter.ConvertToApiFundingSource(FundingSource.Levy))
                       || v.Equals(FundingSourceConverter.ConvertToApiFundingSource(FundingSource.Transfer)));

        RuleFor(m => m.Ukprn).GreaterThan(0);

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
    }
}