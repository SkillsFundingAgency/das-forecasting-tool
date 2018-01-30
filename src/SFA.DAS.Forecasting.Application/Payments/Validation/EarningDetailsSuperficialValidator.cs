using System.Collections.Generic;
using SFA.DAS.Forecasting.Core.Validation;
using SFA.DAS.Forecasting.Application.Payments.Messages;

namespace SFA.DAS.Forecasting.Application.Payments.Validation
{
    public class EarningDetailsSuperficialValidator : ISuperficialValidation<EarningDetails>
    {
        public List<ValidationFailure> Validate(EarningDetails itemToValidate)
        {
	        var failures = new List<ValidationFailure>();
			if (itemToValidate.StartDate == System.DateTime.MinValue) // What is a valid date?
			{
				failures.Add(new ValidationFailure { Reason = $"{nameof(itemToValidate.StartDate)} is not a valid date." });
			}

	        if (itemToValidate.PlannedEndDate == System.DateTime.MinValue) // What is a valid date?
	        {
		        failures.Add(new ValidationFailure { Reason = $"{nameof(itemToValidate.PlannedEndDate)} is not a valid date." });
	        }

	        if (itemToValidate.ActualEndDate == System.DateTime.MinValue) // What is a valid date?
	        {
		        failures.Add(new ValidationFailure { Reason = $"{nameof(itemToValidate.ActualEndDate)} is not a valid date." });
			}

	        if (itemToValidate.CompletionAmount < 0)
	        {
		        failures.Add(new ValidationFailure { Reason = $"{nameof(itemToValidate.CompletionAmount)} is less than zero." });
			}

			if (itemToValidate.CompletionStatus < 0)
			{
				failures.Add(new ValidationFailure { Reason = $"{nameof(itemToValidate.CompletionStatus)} is less than zero." });
			}

	        if (itemToValidate.MonthlyInstallment < 0)
	        {
		        failures.Add(new ValidationFailure { Reason = $"{nameof(itemToValidate.MonthlyInstallment)} is less than zero." });
	        }

	        if (itemToValidate.TotalInstallments < 0)
	        {
		        failures.Add(new ValidationFailure { Reason = $"{nameof(itemToValidate.TotalInstallments)} is less than zero." });
	        }

	        if (string.IsNullOrWhiteSpace(itemToValidate.EndpointAssessorId))
	        {
		        failures.Add(new ValidationFailure { Reason = $"{nameof(itemToValidate.EndpointAssessorId)} is missing." });
	        }

			return failures;
		}
    }
}