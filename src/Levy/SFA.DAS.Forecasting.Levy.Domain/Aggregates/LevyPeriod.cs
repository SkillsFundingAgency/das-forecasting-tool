using SFA.DAS.Forecasting.Levy.Domain.Repositories;
using SFA.DAS.NLog.Logger;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Levy.Domain.Model;
using SFA.DAS.Forecasting.Levy.Domain.Validation;

namespace SFA.DAS.Forecasting.Levy.Domain.Aggregates
{
    public interface ILevyPeriod
    {
        void AddDeclaration(long employerAccountId, string payrollYear, short payrollMonth, decimal amount, string scheme, DateTime transactionDate);
    }

    public class LevyPeriod : ILevyPeriod
    {
        internal readonly List<LevyDeclaration> LevyDeclarations;
        private readonly LevyDeclarationTransactionDateValidator _levyDeclarationTransactionDateValidator;

        //TODO: should really be internal but unit tests need access to contructor. 
        public LevyPeriod()
        {
            LevyDeclarations = new List<LevyDeclaration>();
            _levyDeclarationTransactionDateValidator = new LevyDeclarationTransactionDateValidator();
        }

        public virtual void AddDeclaration(long employerAccountId, string payrollYear, short payrollMonth, decimal amount, string scheme, DateTime transactionDate)
        {
            if (!_levyDeclarationTransactionDateValidator.IsValid(transactionDate))
                throw new InvalidOperationException($"Found LevyDeclarationEvent older than 2 years. Not saved. Employer: {employerAccountId}, Payroll date: {payrollYear} - {payrollMonth}, Amount: {amount}");

            LevyDeclarations.Add(new LevyDeclaration
            {
                EmployerAccountId = employerAccountId,
                Amount = amount,
                Scheme = scheme,
                PayrollYear = payrollYear,
                PayrollMonth =  payrollMonth,
                TransactionDate = transactionDate
            });
        }

        public decimal GetPeriodAmount()
        {
            return LevyDeclarations.Sum(levyDeclaration => levyDeclaration.Amount);
        }
    }

}