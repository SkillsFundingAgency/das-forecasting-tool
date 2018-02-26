using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Forecasting.Domain.Levy.Validation;
using SFA.DAS.Forecasting.Domain.Shared;
using SFA.DAS.Forecasting.Models.Levy;

namespace SFA.DAS.Forecasting.Domain.Levy
{
    public class LevyPeriod
    {
        private readonly IPayrollDateService _payrollDateService;
        internal readonly List<LevyDeclaration> LevyDeclarations;
        private readonly LevyDeclarationTransactionDateValidator _levyDeclarationTransactionDateValidator;

        //TODO: should really be internal but unit tests need access to constructor. 
        public LevyPeriod(IPayrollDateService payrollDateService)
        {
            _payrollDateService = payrollDateService ?? throw new ArgumentNullException(nameof(payrollDateService));
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
                    PayrollDate = _payrollDateService.GetPayrollDate(payrollYear, payrollMonth),

                });
            }

            levyDeclaration.TransactionDate = transactionDate;
            levyDeclaration.LevyAmountDeclared = amount;
            levyDeclaration.DateReceived = DateTime.UtcNow;
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