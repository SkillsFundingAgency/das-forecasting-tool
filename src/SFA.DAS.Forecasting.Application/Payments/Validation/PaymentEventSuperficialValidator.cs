using System.Collections.Generic;
using SFA.DAS.Forecasting.Core.Validation;
using SFA.DAS.Forecasting.Application.Payments.Messages;

namespace SFA.DAS.Forecasting.Application.Payments.Validation
{
    public class PaymentEventSuperficialValidator: ISuperficialValidation<PaymentEvent>
    {
        public List<ValidationFailure> Validate(PaymentEvent itemToValidate)
        {
	        var failures = new List<ValidationFailure>();
	        if (string.IsNullOrWhiteSpace(itemToValidate.EmployerAccountId))
	        {
		        failures.Add(new ValidationFailure { Reason = $"{nameof(itemToValidate.EmployerAccountId)} is missing." });
	        }

	        if (itemToValidate.Amount < 0)
	        {
		        failures.Add(new ValidationFailure { Reason = $"{nameof(itemToValidate.Amount)} is less than zero." });
	        }

	        if (itemToValidate.ApprenticeshipId < 0)
	        {
		        failures.Add(new ValidationFailure { Reason = $"{nameof(itemToValidate.ApprenticeshipId)} is less than zero." });
	        }

	        if (itemToValidate.Ukprn < 0)
	        {
		        failures.Add(new ValidationFailure { Reason = $"{nameof(itemToValidate.Ukprn)} is less than zero." });
	        }

	        var earningsValidation = new EarningDetailsSuperficialValidator().Validate(itemToValidate.EarningDetails);

			failures.AddRange(earningsValidation);

			var collectionPeriodValidation = new CollectionPeriodSuperficialValidator().Validate(itemToValidate.CollectionPeriod);

			failures.AddRange(collectionPeriodValidation);

			return failures;
		}
    }
}