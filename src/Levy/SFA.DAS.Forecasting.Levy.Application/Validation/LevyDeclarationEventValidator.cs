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
            if (itemToValidate.EmployerAccountId < 1)
            {
                failures.Add(new ValidationFailure { Reason = $"{nameof(itemToValidate.EmployerAccountId)} is not valid." });
            }

            if (itemToValidate.Amount < 0)
            {
                failures.Add(new ValidationFailure { Reason = $"{nameof(itemToValidate.Amount)} is less than zero." });
            }

            if (string.IsNullOrWhiteSpace(itemToValidate.Scheme))
            {
                failures.Add(new ValidationFailure { Reason = $"{nameof(itemToValidate.Amount)} is less than zero." });
            }

            if (itemToValidate.PayrollDate == System.DateTime.MinValue)
            {
                failures.Add(new ValidationFailure { Reason = $"{nameof(itemToValidate.PayrollDate)} is not a valid date." });
            }

            if (itemToValidate.TransactionDate == System.DateTime.MinValue)
            {
                failures.Add(new ValidationFailure { Reason = $"{nameof(itemToValidate.TransactionDate)} is not a valid date." });
            }

            if (itemToValidate.TransactionDate < System.DateTime.Now.AddMonths(-25) )
            {
                failures.Add(new ValidationFailure { Reason = $"{nameof(itemToValidate.TransactionDate)} must not be older than 2 years." });
            }

            return failures;
        }
    }
}