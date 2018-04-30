using System;
using SFA.DAS.Forecasting.Domain.Levy.Validation;
using SFA.DAS.Forecasting.Domain.Shared;
using SFA.DAS.Forecasting.Models.Levy;

namespace SFA.DAS.Forecasting.Domain.Levy
{
    public class LevyDeclaration
    {
        private readonly IPayrollDateService _payrollDateService;
        private readonly LevyDeclarationTransactionDateValidator _levyDeclarationTransactionDateValidator;
        internal LevyDeclarationModel Model { get; private set; }

        public long Id => Model.Id;
        public long EmployerAccountId => Model.EmployerAccountId;
        public string Scheme => Model.Scheme;
        public string PayrollYear => Model.PayrollYear;
        public byte PayrollMonth => Model.PayrollMonth;
        public DateTime PayrollDate => Model.PayrollDate;
        public decimal LevyAmountDeclared => Model.LevyAmountDeclared;
        public DateTime TransactionDate => Model.TransactionDate;
        public DateTime DateReceived => Model.DateReceived;

        public LevyDeclaration(IPayrollDateService payrollDateService, LevyDeclarationModel model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            _payrollDateService = payrollDateService ?? throw new ArgumentNullException(nameof(payrollDateService));
            _levyDeclarationTransactionDateValidator = new LevyDeclarationTransactionDateValidator();
        }

        public virtual bool RegisterLevyDeclaration(decimal amount, DateTime transactionDate)
        {
            if (!_levyDeclarationTransactionDateValidator.IsValid(transactionDate))
                throw new InvalidOperationException($"Found LevyDeclarationEvent older than 2 years. Not saved. Employer: {EmployerAccountId}, Payroll date: {PayrollYear} - {PayrollMonth}, Amount: {amount}");

            //TODO: publish validation failure event
            if (transactionDate < Model.TransactionDate)
                return false;
            if (Model.Id <= 0)
                Model.PayrollDate = _payrollDateService.GetPayrollDate(Model.PayrollYear, Model.PayrollMonth);
            Model.TransactionDate = transactionDate;
            Model.LevyAmountDeclared = amount;
            Model.DateReceived = DateTime.UtcNow;
            return true;
        }
    }
}