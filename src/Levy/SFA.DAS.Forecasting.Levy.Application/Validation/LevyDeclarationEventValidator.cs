using System.Collections.Generic;
using SFA.DAS.Forecasting.Core.Validation;
using SFA.DAS.Forecasting.Levy.Application.Messages;

namespace SFA.DAS.Forecasting.Levy.Application.Validation
{
    public class LevyDeclarationEventValidator: ISuperficialValidation<LevyDeclarationEvent>
    {
        public List<ValidationFailure> Validate(LevyDeclarationEvent itemToValidate)
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

            if (string.IsNullOrWhiteSpace(itemToValidate.Scheme))
            {
                failures.Add(new ValidationFailure { Reason = $"{nameof(itemToValidate.Amount)} is less than zero." });
            }

            if (itemToValidate.SubmissionDate == System.DateTime.MinValue) // What is a valid date?
            {
                failures.Add(new ValidationFailure { Reason = $"{nameof(itemToValidate.SubmissionDate)} is not a valid date." });
            }

            if (itemToValidate.TransactionDate == System.DateTime.MinValue) // What is a valid date?
            {
                failures.Add(new ValidationFailure { Reason = $"{nameof(itemToValidate.TransactionDate)} is not a valid date." });
            }

            return failures;
        }
    }
}