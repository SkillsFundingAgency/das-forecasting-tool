using System.Collections.Generic;
using SFA.DAS.Forecasting.Core.Validation;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Application.Payments.Validation
{
    public class PaymentEventSuperficialValidator: ISuperficialValidation<PaymentCreatedMessage>
    {
        public List<ValidationFailure> Validate(PaymentCreatedMessage itemToValidate)
        {
	        var failures = new List<ValidationFailure>();
	        if (string.IsNullOrWhiteSpace(itemToValidate.EmployerAccountId.ToString()))
	        {
		        failures.Add(new ValidationFailure { Reason = $"{nameof(itemToValidate.EmployerAccountId)} is missing." });
	        }
            else if (!long.TryParse(itemToValidate.EmployerAccountId.ToString(), out long res) )
	        {
                failures.Add(new ValidationFailure { Reason = $"{nameof(itemToValidate.EmployerAccountId)} is not a valid employer account id." });
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

            if (!(itemToValidate.FundingSource == FundingSource.Levy || itemToValidate.FundingSource == FundingSource.Transfer) )
            {
                failures.Add(new ValidationFailure { Reason = $"{nameof(itemToValidate.FundingSource)} is not valid." });
            }

            var earningsValidation = new EarningDetailsSuperficialValidator().Validate(itemToValidate.EarningDetails);

			failures.AddRange(earningsValidation);

			var collectionPeriodValidation = new CollectionPeriodSuperficialValidator().Validate(itemToValidate.CollectionPeriod);

			failures.AddRange(collectionPeriodValidation);

			return failures;
		}
    }
}