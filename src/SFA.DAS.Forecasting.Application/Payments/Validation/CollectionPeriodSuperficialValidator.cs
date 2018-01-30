using System.Collections.Generic;
using SFA.DAS.Forecasting.Core.Validation;
using SFA.DAS.Forecasting.Application.Payments.Messages;

namespace SFA.DAS.Forecasting.Application.Payments.Validation
{
    public class CollectionPeriodSuperficialValidator : ISuperficialValidation<CollectionPeriod>
    {
        public List<ValidationFailure> Validate(CollectionPeriod itemToValidate)
        {
	        var failures = new List<ValidationFailure>();

			if (string.IsNullOrWhiteSpace(itemToValidate.Id))
	        {
		        failures.Add(new ValidationFailure { Reason = $"{nameof(itemToValidate.Id)} is missing." });
	        }

			if (itemToValidate.Month <= 0 || itemToValidate.Month >= 12)
			{
				failures.Add(new ValidationFailure { Reason = $"{nameof(itemToValidate.Month)} is invalid." });
			}

	        if (itemToValidate.Year < 0)
	        {
		        failures.Add(new ValidationFailure { Reason = $"{nameof(itemToValidate.Month)} is less than zero." });
	        }

			return failures;
		}
    }
}