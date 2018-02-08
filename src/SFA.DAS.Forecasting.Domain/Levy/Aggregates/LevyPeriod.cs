using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Forecasting.Domain.Levy.Model;
using SFA.DAS.Forecasting.Domain.Levy.Validation;

namespace SFA.DAS.Forecasting.Domain.Levy.Aggregates
{
    public class LevyPeriod
    {
        internal readonly List<LevyDeclaration> LevyDeclarations;
        private readonly LevyDeclarationTransactionDateValidator _levyDeclarationTransactionDateValidator;

        //TODO: should really be internal but unit tests need access to contructor. 
        public LevyPeriod()
        {
            LevyDeclarations = new List<LevyDeclaration>();
            _levyDeclarationTransactionDateValidator = new LevyDeclarationTransactionDateValidator();
        }

        public virtual void AddDeclaration(long employerAccountId, string payrollYear, byte payrollMonth, decimal amount, string scheme, DateTime transactionDate)
        {
            if (!_levyDeclarationTransactionDateValidator.IsValid(transactionDate))
                throw new InvalidOperationException($"Found LevyDeclarationEvent older than 2 years. Not saved. Employer: {employerAccountId}, Payroll date: {payrollYear} - {payrollMonth}, Amount: {amount}");

            var levyDeclaration = LevyDeclarations.FirstOrDefault(levy =>
                levy.PayrollMonth == payrollMonth && levy.PayrollYear == payrollYear && levy.Scheme.Equals(scheme));
            if (levyDeclaration == null)
            {
                LevyDeclarations.Add(levyDeclaration = new LevyDeclaration
                {
                    EmployerAccountId = employerAccountId,
                    Scheme = scheme,
                    PayrollYear = payrollYear,
                    PayrollMonth = payrollMonth,
                });
            }

            levyDeclaration.TransactionDate = transactionDate;
            levyDeclaration.LevyAmountDeclared = amount;
        }

        public decimal GetPeriodAmount()
        {
            return LevyDeclarations.Sum(levyDeclaration => levyDeclaration.LevyAmountDeclared);
        }

        public DateTime? GetLastTimeReceivedLevy()
        {
            return LevyDeclarations.OrderByDescending(levy => levy.DateReceived)
                .FirstOrDefault()
                ?.DateReceived;
        }
    }
}