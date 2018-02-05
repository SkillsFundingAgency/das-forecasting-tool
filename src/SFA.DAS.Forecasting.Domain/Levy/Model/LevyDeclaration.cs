using System;

namespace SFA.DAS.Forecasting.Domain.Levy.Model
{
    public class LevyDeclaration
    {
        public long Id { get; set; }
        public long EmployerAccountId { get; set; }
        public string Scheme { get; set; }
        public string PayrollYear { get; set; }
        public byte PayrollMonth { get; set; }
        public decimal LevyAmountDeclared { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime DateReceived { get; set; }
    }
}