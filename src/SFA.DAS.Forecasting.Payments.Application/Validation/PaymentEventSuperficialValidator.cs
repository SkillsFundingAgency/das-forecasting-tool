using System.Collections.Generic;
using SFA.DAS.Forecasting.Core.Validation;
using SFA.DAS.Forecasting.Payments.Application.Messages;

namespace SFA.DAS.Forecasting.Payments.Application.Validation
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

			return failures;
		}
    }
}