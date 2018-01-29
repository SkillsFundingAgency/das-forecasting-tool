using System;

namespace SFA.DAS.Forecasting.Levy.Domain.Validation
{
    public class LevyDeclarationTransactionDateValidator
    {
        public bool IsValid(DateTime transactionDate)
        {
            return transactionDate > DateTime.Today.AddMonths(-25);
        }
    }
}